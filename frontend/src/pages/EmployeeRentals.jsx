import React, { useEffect, useState } from "react";
import "../styles/EmployeeRentals.css";
import { useNavigate } from "react-router-dom";

export default function EmployeeRentals() {
  const [rentals, setRentals] = useState([]);
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
        {rental.customerName || rental.userName || rental.guestName || "Ismeretlen"}
      </p>

      <p>
        <strong>Email:</strong>{" "}
        {rental.customerEmail || rental.userEmail || rental.guestEmail || "-"}
      </p>

      <p>
        <strong>Időszak:</strong>{" "}
        {new Date(rental.startDate).toLocaleDateString()} -{" "}
        {new Date(rental.endDate).toLocaleDateString()}
      </p>

      <p>
        <strong>Státusz:</strong>{" "}
        {rental.statusText || rental.status}
      </p>

      {(rental.statusText === "Requested" || rental.status === 0 || rental.status === "Requested") && (
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
    </div>
  </div>
))}
      </div>
    </div>
  );
}