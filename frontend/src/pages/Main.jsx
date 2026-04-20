import React from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Main.css";

export default function MainPage() {
  const navigate = useNavigate();

  const token = localStorage.getItem("token");

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

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
              <Link to="/profile" className="profile-icon" title="Profil">
                👤
              </Link>

              <button onClick={handleLogout} className="logout-btn">
                Logout
              </button>
            </>
          )}

        </div>
      </nav>

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
    </div>
  );
}