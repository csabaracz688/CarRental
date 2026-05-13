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

  //  CSAK ELÉRHETŐ AUTÓK
  const availableCars = useMemo(() => {
    return cars.filter((car) => car.status === 0);
  }, [cars]);

  const brands = useMemo(() => {
    return [...new Set(availableCars.map((car) => car.brand).filter(Boolean))];
  }, [availableCars]);

  const models = useMemo(() => {
    return [...new Set(availableCars.map((car) => car.model).filter(Boolean))];
  }, [availableCars]);

  const filteredCars = useMemo(() => {
    return availableCars.filter((car) => {
      const search = searchTerm.toLowerCase();

      const matchesSearch =
        car.brand?.toLowerCase().includes(search) ||
        car.model?.toLowerCase().includes(search) ||
        car.licensePlate?.toLowerCase().includes(search);

      const matchesBrand = brandFilter ? car.brand === brandFilter : true;
      const matchesModel = modelFilter ? car.model === modelFilter : true;

      return matchesSearch && matchesBrand && matchesModel;
    });
  }, [availableCars, searchTerm, brandFilter, modelFilter]);

  const featuredCar = useMemo(() => {
    return availableCars.length > 0 ? availableCars[0] : null;
  }, [availableCars]);

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

                  <Link to="/profile" className="profile-icon">
                    Profile
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
        <div className="hero-inner">
          <div className="hero-left">
            <h2>Find your perfect ride</h2>
            <p>
              Fast, reliable car rentals with transparent pricing. Browse our
              available fleet and book in minutes.
            </p>

            <div className="hero-ctas">
              <Link to="/" className="btn btn-primary" aria-label="Browse cars">
                Browse cars
              </Link>

              {!token && (
                <Link to="/register" className="btn btn-ghost" aria-label="Create account">
                  Create account
                </Link>
              )}
            </div>
          </div>

          <div className="hero-right">
            {featuredCar ? (
              <div className="hero-featured" onClick={() => navigate(`/cars/${featuredCar.id}`)}>
                <img src={featuredCar.imageUrl ? `https://localhost:7077${featuredCar.imageUrl}`
      : "https://via.placeholder.com/400x250"} 
      alt={`${featuredCar.brand} ${featuredCar.model}`} />
                <div className="featured-badge">{featuredCar.brand} {featuredCar.model}</div>
              </div>
            ) : (
              <div className="hero-featured placeholder">
                <p>No featured car</p>
              </div>
            )}
          </div>
        </div>
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

          <button
            className="clear-filters-btn"
            onClick={() => {
              setSearchTerm("");
              setBrandFilter("");
              setModelFilter("");
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
                  src={car.imageUrl ? `https://localhost:7077${car.imageUrl}`
      : "https://via.placeholder.com/400x250"}
                  alt={`${car.brand} ${car.model}`}
                />

                <h3>{car.brand} {car.model}</h3>
                <p>{car.dailyPrice} Ft/day</p>

                <p className="car-status">Available</p>
              </div>
            ))
          )}
        </div>
      </section>
    </div>
  );
}