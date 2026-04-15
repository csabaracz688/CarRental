import { Link, useNavigate } from "react-router-dom";
import "../styles/EmployeeDashboard.css";

export default function EmployeeDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <div className="container">
      
      {/* HEADER */}
      <div className="header">
        <h1>Ügyintéző Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Kijelentkezés
        </button>
      </div>

      {/* KÁRTYÁK */}
      <div className="cards">

        <div className="card">
          <h2>Kölcsönzések</h2>
          <p>Aktív és korábbi bérlések</p>
          <Link to="/employee/rentals">
            <button>Megnyitás</button>
          </Link>
        </div>

        <div className="card">
          <h2>Autók</h2>
          <p>Elérhető autók megtekintése</p>
          <Link to="/">
            <button>Megtekintés</button>
          </Link>
        </div>

      </div>
    </div>
  );
}