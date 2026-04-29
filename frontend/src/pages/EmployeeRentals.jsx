import React, { useEffect, useState } from "react";
import "../styles/EmployeeRentals.css";
import { useNavigate } from "react-router-dom";

export default function EmployeeRentals() {
  const [rentals, setRentals] = useState([]);
  const [handoverDates, setHandoverDates] = useState({});
  const token = localStorage.getItem("token");
  const navigate = useNavigate();

  const loadRentals = async () => {
    try {
      const res = await fetch("https://localhost:7077/api/rentals", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const text = await res.text();

      if (!res.ok) {
        console.error("Backend error:", res.status, text);
        return;
      }

      setRentals(text ? JSON.parse(text) : []);
    } catch (err) {
      console.error("Rentals loading error:", err);
    }
  };

  useEffect(() => {
    loadRentals();
  }, []);

  const approveRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/approve`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      alert("Approve failed!");
      return;
    }

    loadRentals();
  };

  const rejectRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/reject`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      alert("Reject failed!");
      return;
    }

    loadRentals();
  };

  const handOverRental = async (id) => {
    const rawDate = handoverDates[String(id)];

    if (!rawDate) {
      alert("Add meg az átadás dátumát!");
      return;
    }

    const res = await fetch(`https://localhost:7077/api/rentals/${id}/handover`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        handedOverAt: rawDate,
      }),
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Handover error:", res.status, text);
      alert("Átadás sikertelen!");
      return;
    }

    alert("Átadás rögzítve!");
    loadRentals();
  };

  const closeRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/close`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Close error:", res.status, text);
      alert("Close failed!");
      return;
    }

    alert("Rental closed!");
    loadRentals();
  };

  return (
    <div className="employee-rentals-page">
      <button className="back-btn" onClick={() => navigate("/employee")}>
        ← Back
      </button>

      <h1>Bérlések</h1>
      <p>Aktív és korábbi bérlések áttekintése.</p>

      <div className="employee-rentals-list">
        {rentals.map((rental) => (
          <div key={rental.id} className="employee-rental-card">
            <img
              className="employee-rental-image"
              src={rental.imageUrl || "https://via.placeholder.com/300x180"}
              alt={`${rental.carBrand} ${rental.carModel}`}
            />

            <div className="employee-rental-info">
              <h2>
                {rental.carBrand} {rental.carModel}
              </h2>

              <p>
                <strong>Rendszám:</strong> {rental.licensePlate}
              </p>

              <p>
                <strong>Bérlő:</strong>{" "}
                {rental.customerName ||
                  rental.userName ||
                  rental.guestName ||
                  "Ismeretlen"}
              </p>

              <p>
                <strong>Email:</strong>{" "}
                {rental.customerEmail ||
                  rental.userEmail ||
                  rental.guestEmail ||
                  "-"}
              </p>

              <p>
                <strong>Időszak:</strong>{" "}
                {new Date(rental.startDate).toLocaleDateString()} -{" "}
                {new Date(rental.endDate).toLocaleDateString()}
              </p>

              <p>
                <strong>Státusz:</strong> {rental.statusText || rental.status}
              </p>

              <p>
                <strong>Átadás:</strong>{" "}
                {rental.handedOverAt
                  ? new Date(rental.handedOverAt).toLocaleString()
                  : "Még nincs átadva"}
              </p>

              <p>
                <strong>Visszahozás / lezárás:</strong>{" "}
                {rental.closedAt
                  ? new Date(rental.closedAt).toLocaleString()
                  : "Még nincs lezárva"}
              </p>

              {(rental.statusText === "Requested" ||
                rental.status === 0 ||
                rental.status === "Requested") && (
                <div className="employee-rental-actions">
                  <button
                    className="approve-btn"
                    onClick={() => approveRental(rental.id)}
                  >
                    Accept
                  </button>

                  <button
                    className="reject-btn"
                    onClick={() => rejectRental(rental.id)}
                  >
                    Decline
                  </button>
                </div>
              )}

              {(rental.statusText === "Approved" ||
                rental.status === "Approved") &&
                !rental.handedOverAt && (
                  <div className="handover-box">
                    <input
                      type="datetime-local"
                      value={handoverDates[String(rental.id)] || ""}
                      onChange={(e) =>
                        setHandoverDates((prev) => ({
                          ...prev,
                          [String(rental.id)]: e.target.value,
                        }))
                      }
                    />

                    <button
                      className="handover-btn"
                      onClick={() => handOverRental(rental.id)}
                    >
                      Átadás rögzítése
                    </button>
                  </div>
                )}

              {rental.handedOverAt && !rental.closedAt && (
                <button
                  className="close-btn"
                  onClick={() => closeRental(rental.id)}
                >
                  Kölcsönzés lezárása
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}