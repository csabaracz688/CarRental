import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import "../styles/CarDetails.css";

export default function CarDetails() {
  const { id } = useParams();

  const [car, setCar] = useState(null);
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [availability, setAvailability] = useState(null);

  useEffect(() => {
    fetch(`https://localhost:7077/api/cars/${id}`)
      .then(res => res.json())
      .then(setCar);
  }, [id]);

  const checkAvailability = () => {
    fetch(`https://localhost:7077/api/cars/${id}/availability?start=${start}&end=${end}`)
      .then(res => res.json())
      .then(setAvailability);
  };

  const handleBooking = () => {
    if (!availability?.isAvailable) {
      alert("This car is not available!");
      return;
    }

    alert("Booking successful!");
  };

  if (!car) return <p>Loading...</p>;

  return (
    <div className="car-details">

      <div className="car-details-card">

        <img
          className="car-details-image"
          src={car.imageUrl || "https://via.placeholder.com/400x250"}
          alt=""
        />

        <div className="car-details-content">

          <h2>{car.brand} {car.model}</h2>

          <p className="price">{car.dailyPrice} €/day</p>

          <p>Kilometers: {car.distanceKm}</p>

          <div className="booking-section">
            <h3>Booking</h3>

            <input
              type="date"
              value={start}
              onChange={(e) => setStart(e.target.value)}
            />

            <input
              type="date"
              value={end}
              onChange={(e) => setEnd(e.target.value)}
            />

            <button className="book-btn" onClick={checkAvailability}>
              Check availability
            </button>

            {availability && (
              <p className="availability">
                {availability.isAvailable
                  ? "Available ✅"
                  : "Not available ❌"}
              </p>
            )}

            <button className="book-btn" onClick={handleBooking}>
              Book now
            </button>

          </div>

        </div>
      </div>
    </div>
  );
}