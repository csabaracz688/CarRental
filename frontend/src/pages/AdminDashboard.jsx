import { Link, useNavigate } from "react-router-dom";
import "../styles/Dashboard.css";

export default function AdminDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <div className="container">
      
      <div className="header">
        <h1>Admin Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </div>

      <div className="cards">
        
        <div className="card">
          <h2>Cars</h2>
          <p>Manage all vehicles</p>
          <Link to="/admin/cars">
            <button>Open</button>
          </Link>
        </div>

        <div className="card">
          <h2>Add Car</h2>
          <p>Create a new vehicle entry</p>
          <Link to="/admin/add-car">
            <button>Add</button>
          </Link>
        </div>

      </div>
    </div>
  );
}