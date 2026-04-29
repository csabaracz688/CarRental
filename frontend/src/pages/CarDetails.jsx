import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "../styles/CarDetails.css";

export default function CarDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const [car, setCar] = useState(null);
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [availability, setAvailability] = useState(null);
  const [loading, setLoading] = useState(false);

  const [guestName, setGuestName] = useState("");
  const [guestEmail, setGuestEmail] = useState("");
  const [guestPhone, setGuestPhone] = useState("");

  useEffect(() => {
    fetch(`https://localhost:7077/api/cars/${id}`)
      .then((res) => res.json())
      .then(setCar)
      .catch((err) => console.error("Car loading error:", err));
  }, [id]);

  const checkAvailability = async () => {
    if (!start || !end) {
      alert("Please select dates!");
      return false;
    }

    if (start >= end) {
      alert("End date must be after start date!");
      return false;
    }

    const res = await fetch(
      `https://localhost:7077/api/cars/${id}/availability?start=${start}&end=${end}`
    );

    const data = await res.json();
    setAvailability(data);

    return data.isAvailable;
  };

  const handleBooking = async () => {
    setLoading(true);

    try {
      const isAvailable = await checkAvailability();

      if (!isAvailable) {
        alert("Not available!");
        return;
      }

      if (!token && (!guestName || !guestEmail || !guestPhone)) {
        alert("Please fill all guest fields!");
        return;
      }

      const body = {
        carId: Number(id),
        startDate: start,
        endDate: end,
      };

      if (!token) {
        body.guestName = guestName;
        body.guestEmail = guestEmail;
        body.guestPhone = guestPhone;
      }

      const res = await fetch("https://localhost:7077/api/rentals/request", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...(token && { Authorization: `Bearer ${token}` }),
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        const error = await res.json().catch(() => null);
        alert(error?.message || "Booking failed!");
        return;
      }

      alert("Booking request sent!");
      navigate("/");
    } catch (err) {
      console.error("Booking error:", err);
      alert("Something went wrong!");
    } finally {
      setLoading(false);
    }
  };

  if (!car) return <p>Loading...</p>;

  return (
    <div className="car-details">
      <button className="back-btn" onClick={() => navigate("/")}>
        ← Back
      </button>

      <div className="car-details-card">
        <img
          className="car-details-image"
          src={car.imageUrl || "https://via.placeholder.com/400x250"}
          alt={`${car.brand} ${car.model}`}
        />

        <div className="car-details-content">
          <h2>
            {car.brand} {car.model}
          </h2>

          <p className="price">{car.dailyPrice} Ft / day</p>
          <p>Kilometers: {car.distanceKm}</p>

          <div className="booking-section">
            <h3>Booking</h3>

            <input
              type="date"
              value={start}
              onChange={(e) => {
                setStart(e.target.value);
                setAvailability(null);
              }}
            />

            <input
              type="date"
              value={end}
              onChange={(e) => {
                setEnd(e.target.value);
                setAvailability(null);
              }}
            />

            {!token && (
              <>
                <input
                  type="text"
                  placeholder="Name"
                  value={guestName}
                  onChange={(e) => setGuestName(e.target.value)}
                />

                <input
                  type="email"
                  placeholder="Email"
                  value={guestEmail}
                  onChange={(e) => setGuestEmail(e.target.value)}
                />

                <input
                  type="text"
                  placeholder="Phone"
                  value={guestPhone}
                  onChange={(e) => setGuestPhone(e.target.value)}
                />
              </>
            )}

            <button className="book-btn" onClick={checkAvailability}>
              Check availability
            </button>

            {availability && (
              <p className="availability">
                {availability.isAvailable
                  ? "Available ✅"
                  : `Not available ❌ ${availability.reason || ""}`}
              </p>
            )}

            <button
              className="book-btn"
              onClick={handleBooking}
              disabled={loading}
            >
              {loading ? "Booking..." : "Book now"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}