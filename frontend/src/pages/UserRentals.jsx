import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/UserRentals.css";

export default function UserRentals() {
  const [rentals, setRentals] = useState([]);
  const token = localStorage.getItem("token");
  const navigate = useNavigate();

  useEffect(() => {
    const loadRentals = async () => {
      try {
        if (!token) {
          console.error("No token found");
          navigate("/login");
          return;
        }

        const res = await fetch("https://localhost:7077/api/rentals/user-rentals", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const text = await res.text();

        if (!res.ok) {
          console.error("Backend error:", res.status, text);

          if (res.status === 401) {
            localStorage.clear();
            navigate("/login");
          }

          return;
        }

        setRentals(text ? JSON.parse(text) : []);
      } catch (err) {
        console.error("Rentals loading error:", err);
      }
    };

    loadRentals();
  }, [token, navigate]);

  return (
    <div className="user-rentals-page">
      <button className="back-btn" onClick={() => navigate("/")}>
        ← Back
      </button>

      <h1>My Rentals</h1>

      {rentals.length === 0 ? (
        <p className="empty-text">You have no rentals yet.</p>
      ) : (
        <div className="rentals-list">
          {rentals.map((rental) => (
            <div key={rental.id} className="rental-card">
              <img
                className="rental-car-image"
                src={
                  rental.car?.imageUrl ||
                  (rental.car?.imagePath
                    ? `https://localhost:7077/uploads/${rental.car.imagePath}`
                    : "https://via.placeholder.com/300x180")
                }
                alt={`${rental.car?.brand} ${rental.car?.model}`}
              />

              <div className="rental-info">
                <h2>
                  {rental.car?.brand} {rental.car?.model}
                </h2>

                <p>
                  <strong>From:</strong>{" "}
                  {new Date(rental.startDate).toLocaleDateString()}
                </p>

                <p>
                  <strong>To:</strong>{" "}
                  {new Date(rental.endDate).toLocaleDateString()}
                </p>

                <p>
                  <strong>Status:</strong> {rental.status}
                </p>

                <p>
                  <strong>Price/day:</strong> {rental.car?.dailyPrice} Ft
                </p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}