import { Link } from "react-router-dom";

export default function AdminDashboard() {
  return (
    <div>
      <h1>Admin Dashboard</h1>

      <Link to="/admin/cars">Autók kezelése</Link><br />
      <Link to="/admin/add-car">Új autó</Link>
    </div>
  );
}