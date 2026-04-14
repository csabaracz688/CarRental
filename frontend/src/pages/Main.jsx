import React from "react";
import { Link } from "react-router-dom";
import "../styles/Main.css";

export default function MainPage() {
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
          <Link to="/login" className="login-btn">Login</Link>
          <Link to="/register" className="register-btn">Register</Link>
        </div>
      </nav>

      <section className="hero">
        <h2>Rent Cars Easily</h2>
        <p>
          Welcome to our car rental platform. Use the search bar to find cars
          once the admin uploads them.
        </p>

        <div className="hero-buttons">
          <Link to="/login" className="hero-login">Go to Login</Link>
          <Link to="/register" className="hero-register">Create Account</Link>
        </div>
      </section>

    </div>
  );
}