import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Main.css";

export default function MainPage() {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const [cars, setCars] = useState([]);

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

  // 🔥 AUTÓK LEKÉRÉSE BACKENDRŐL
  useEffect(() => {
    fetch("https://localhost:7077/api/cars")
      .then((res) => res.json())
      .then((data) => setCars(data))
      .catch((err) => console.error("Error loading cars:", err));
  }, []);

  return (
    <div className="main-container">

      <nav className="navbar">
        <h1 className="logo">CarRental</h1>

        <div className="search-container">
          <input
            type="text"
            placeholder="Search for cars..."
            className="search-input"
          />
        </div>

        <div className="nav-buttons">

          {!token ? (
            <>
              <Link to="/login" className="login-btn">Login</Link>
              <Link to="/register" className="register-btn">Register</Link>
            </>
          ) : (
            <>
              <Link to="/profile" className="profile-icon" title="Profil" aria-label="Profile">
                👤
              </Link>

              <button onClick={handleLogout} className="logout-btn">
                Logout
              </button>
            </>
          )}

        </div>
      </nav>

      {/* HERO */}
      <section className="hero">
        <h2>Rent Cars Easily</h2>
        <p>
          Welcome to our car rental platform. Use the search bar to find cars
          once the admin uploads them.
        </p>

        {!token && (
          <div className="hero-buttons">
            <Link to="/login" className="hero-login">Go to Login</Link>
            <Link to="/register" className="hero-register">Create Account</Link>
          </div>
        )}
      </section>

      {/* 🚗 AUTÓK LISTÁJA */}
      <section className="cars-section">
        <h2>Available Cars</h2>

        <div className="cars-grid">
          {cars.length === 0 ? (
            <p>No cars available yet.</p>
          ) : (
            cars.map((car) => (
              <div key={car.id} className="car-card" onClick={() => navigate(`/cars/${car.id}`)}>

                <img
                  src={car.imageUrl || "https://via.placeholder.com/300"}
                  alt={`${car.brand} ${car.model}`}
                />
              
                <h3>{car.brand} {car.model}</h3>

                <p>{car.dailyPrice} €/day</p>

                <p className="car-status">
                  {car.status === 0 ? "Available" : "Unavailable"}
                </p>

              </div>
              
              
            ))
          )}
        </div>
      </section>

    </div>
  );
}