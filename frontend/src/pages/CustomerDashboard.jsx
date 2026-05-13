import { Link, useNavigate } from "react-router-dom";
import "../styles/Dashboard.css";

export default function CustomerDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("userId");
    localStorage.removeItem("userName");
    navigate("/");
  };

  return (
    <div className="container">
      <div className="header">
        <h1>Customer Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </div>

      <div className="cards">
        <div className="card">
          <h2>Browse Cars</h2>
          <p>Explore available vehicles and daily prices.</p>
          <Link to="/">
            <button>Open</button>
          </Link>
        </div>

        <div className="card">
          <h2>My Rentals</h2>
          <p>Review your active and past bookings.</p>
          <Link to="/user-rentals">
            <button>Open</button>
          </Link>
        </div>
      </div>
    </div>
  );
}