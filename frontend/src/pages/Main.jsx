import React, { useEffect, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Main.css";

export default function MainPage() {
  const navigate = useNavigate();

  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");
  const isCustomer = role?.toLowerCase() === "customer";

  const [cars, setCars] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [brandFilter, setBrandFilter] = useState("");
  const [modelFilter, setModelFilter] = useState("");
  const [availabilityFilter, setAvailabilityFilter] = useState("");

  const handleLogout = () => {
    localStorage.clear();
    navigate("/");
  };

  useEffect(() => {
    fetch("https://localhost:7077/api/cars")
      .then((res) => res.json())
      .then((data) => setCars(data))
      .catch((err) => console.error("Error loading cars:", err));
  }, []);

  const brands = useMemo(() => {
    return [...new Set(cars.map((car) => car.brand).filter(Boolean))];
  }, [cars]);

  const models = useMemo(() => {
    return [...new Set(cars.map((car) => car.model).filter(Boolean))];
  }, [cars]);

  const filteredCars = useMemo(() => {
    return cars.filter((car) => {
      const search = searchTerm.toLowerCase();

      const matchesSearch =
        car.brand?.toLowerCase().includes(search) ||
        car.model?.toLowerCase().includes(search) ||
        car.licensePlate?.toLowerCase().includes(search);

      const matchesBrand = brandFilter ? car.brand === brandFilter : true;
      const matchesModel = modelFilter ? car.model === modelFilter : true;

      const matchesAvailability =
        availabilityFilter === ""
          ? true
          : availabilityFilter === "available"
          ? car.status === 0
          : car.status !== 0;

      return matchesSearch && matchesBrand && matchesModel && matchesAvailability;
    });
  }, [cars, searchTerm, brandFilter, modelFilter, availabilityFilter]);

  return (
    <div className="main-container">
      <nav className="navbar">
        <h1 className="logo">CarRental</h1>

        <div className="search-container">
          <input
            type="text"
            placeholder="Search for cars..."
            className="search-input"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
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
              {isCustomer && (
                <>
                  <Link to="/user-rentals" className="my-rentals-btn">
                    My rentals
                  </Link>

                  <Link to="/profile" className="profile-icon" title="Profil" aria-label="Profile">
                    👤
                  </Link>
                </>
              )}

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
          Welcome to our car rental platform. Search and filter available cars.
        </p>

        {!token && (
          <div className="hero-buttons">
            <Link to="/login" className="hero-login">Go to Login</Link>
            <Link to="/register" className="hero-register">Create Account</Link>
          </div>
        )}
      </section>

      <section className="cars-section">
        <h2>Available Cars</h2>

        <div className="filters">
          <select value={brandFilter} onChange={(e) => setBrandFilter(e.target.value)}>
            <option value="">All brands</option>
            {brands.map((brand) => (
              <option key={brand} value={brand}>{brand}</option>
            ))}
          </select>

          <select value={modelFilter} onChange={(e) => setModelFilter(e.target.value)}>
            <option value="">All models</option>
            {models.map((model) => (
              <option key={model} value={model}>{model}</option>
            ))}
          </select>

          <select
            value={availabilityFilter}
            onChange={(e) => setAvailabilityFilter(e.target.value)}
          >
            <option value="">All statuses</option>
            <option value="available">Available</option>
            <option value="unavailable">Unavailable</option>
          </select>

          <button
            className="clear-filters-btn"
            onClick={() => {
              setSearchTerm("");
              setBrandFilter("");
              setModelFilter("");
              setAvailabilityFilter("");
            }}
          >
            Clear filters
          </button>
        </div>

        <div className="cars-grid">
          {filteredCars.length === 0 ? (
            <p>No cars found.</p>
          ) : (
            filteredCars.map((car) => (
              <div
                key={car.id}
                className="car-card"
                onClick={() => navigate(`/cars/${car.id}`)}
              >
                <img
                  src={car.imageUrl || "https://via.placeholder.com/300"}
                  alt={`${car.brand} ${car.model}`}
                />

                <h3>{car.brand} {car.model}</h3>
                <p>{car.dailyPrice} Ft/day</p>

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