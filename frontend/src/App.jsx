import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from "./pages/Register";
import Login from "./pages/Login";
import Main from "./pages/Main";
import AdminDashboard from "./pages/AdminDashboard";
import AdminCars from "./pages/AdminCars";
import AdminAddCar from "./pages/AdminAddCar";
import EmployeeDashboard from "./pages/EmployeeDashboard";
import EmployeeRentals from "./pages/EmployeeRentals";
import CustomerDashboard from "./pages/CustomerDashboard";
import ProtectedRoute from "./components/ProtectedRoute";

function App() {
  return (
    <BrowserRouter>
      <Routes>

        {/* PUBLIC */}
        <Route path="/" element={<Main />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* ADMIN */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute role="Admin">
              <AdminDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/cars"
          element={
            <ProtectedRoute role="Admin">
              <AdminCars />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/add-car"
          element={
            <ProtectedRoute role="Admin">
              <AdminAddCar />
            </ProtectedRoute>
          }
        />

        {/* EMPLOYEE */}
        <Route
          path="/employee"
          element={
            <ProtectedRoute role="Officer">
              <EmployeeDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/employee/rentals"
          element={
            <ProtectedRoute role="Officer">
              <EmployeeRentals />
            </ProtectedRoute>
          }
        />

        {/* CUSTOMER */}
        <Route
          path="/customer"
          element={
            <ProtectedRoute role="Customer">
              <CustomerDashboard />
            </ProtectedRoute>
          }
        />

      </Routes>
    </BrowserRouter>
  );
}

export default App;