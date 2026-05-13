import React, { useCallback, useEffect, useState } from "react";
import "../styles/AdminCars.css";
import { useNavigate } from "react-router-dom";
import { useToast } from "../components/useToast";
import { useConfirm } from "../components/useConfirm";

export default function AdminCars() {
  const [cars, setCars] = useState([]);
  const [editingCar, setEditingCar] = useState(null);
  const token = localStorage.getItem("token");
  const navigate = useNavigate();
  const toast = useToast();
  const confirm = useConfirm();

  const loadCars = useCallback(async () => {
    try {
      const res = await fetch("https://localhost:7077/api/cars");

      if (!res.ok) {
        console.error("Cars loading failed:", res.status);
        return;
      }

      const data = await res.json();
      setCars(data);
    } catch (err) {
      console.error("Cars loading error:", err);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    loadCars();
  }, [loadCars]);

  const deleteCar = async (id) => {
  const confirmed = await confirm("Are you sure you want to delete this car?");
  if (!confirmed) return;

  const res = await fetch(`https://localhost:7077/api/cars/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (res.status === 409) {
    const shouldDeactivate = await confirm(
      "This car has an active rental and cannot be deleted. Do you want to deactivate it instead?"
    );

    if (!shouldDeactivate) return;

    const deactivateRes = await fetch(
      `https://localhost:7077/api/cars/${id}/deactivate`,
      {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (!deactivateRes.ok) {
      toast("Deactivation failed!", { type: "error" });
      return;
    }

    toast("Car has been deactivated!", { type: "success" });
    loadCars();
    return;
  }

  if (!res.ok) {
    toast("Delete failed!", { type: "error" });
    return;
  }

  toast("Car deleted!", { type: "success" });
  loadCars();
};

  const startEdit = (car) => {
    setEditingCar({
      id: car.id,
      licensePlate: car.licensePlate || "",
      brand: car.brand || "",
      model: car.model || "",
      distanceKm: car.distanceKm || 0,
      dailyPrice: car.dailyPrice || 0,
      status: car.status ?? 0,
      unavailableFrom: car.unavailableFrom ? car.unavailableFrom.slice(0, 10) : "",
      unavailableTo: car.unavailableTo ? car.unavailableTo.slice(0, 10) : "",
      unavailableReason: car.unavailableReason ?? "",
      unavailableNote: car.unavailableNote || "",
      image: null,
      currentImageUrl: car.imageUrl || "",
      previewImageUrl: car.imageUrl || "",
    });
  };

  const handleEditChange = (e) => {
    const { name, value } = e.target;

    setEditingCar((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];

    if (!file) return;

    setEditingCar((prev) => ({
      ...prev,
      image: file,
      previewImageUrl: URL.createObjectURL(file),
    }));
  };

  const saveEdit = async (e) => {
    e.preventDefault();

    const formData = new FormData();

    formData.append("Id", editingCar.id);
    formData.append("LicensePlate", editingCar.licensePlate);
    formData.append("Brand", editingCar.brand);
    formData.append("Model", editingCar.model);
    formData.append("DistanceKm", Number(editingCar.distanceKm));
    formData.append("DailyPrice", Number(editingCar.dailyPrice));
    formData.append("Status", Number(editingCar.status));

    if (editingCar.unavailableFrom) {
      formData.append("UnavailableFrom", editingCar.unavailableFrom);
    }

    if (editingCar.unavailableTo) {
      formData.append("UnavailableTo", editingCar.unavailableTo);
    }

    if (editingCar.unavailableReason !== "") {
      formData.append("UnavailableReason", Number(editingCar.unavailableReason));
    }

    if (editingCar.unavailableNote) {
      formData.append("UnavailableNote", editingCar.unavailableNote);
    }

    if (editingCar.image) {
      formData.append("Image", editingCar.image);
    }

    const res = await fetch(`https://localhost:7077/api/cars/${editingCar.id}`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Update error:", res.status, text);
      toast("Update failed!", { type: "error" });
      return;
    }

    toast("Car updated!", { type: "success" });
    setEditingCar(null);
    loadCars();
  };

  const getStatusText = (status) => {
    switch (Number(status)) {
      case 0:
        return "Available";
      case 1:
        return "Unavailable";
      case 2:
        return "Rented";
      default:
        return "Unknown";
    }
  };

  return (
    <div className="admin-cars-page">
      <button className="back-btn" onClick={() => navigate("/admin")}>
        ← Back
      </button>
      <h1>Admin Cars</h1>
      <p>View, edit, remove, and manage car status.</p>

      {editingCar && (
        <div className="edit-modal-overlay">
          <div className="edit-modal">
            <h2>Edit Car</h2>

            <form onSubmit={saveEdit}>
              <input
                name="licensePlate"
                placeholder="License plate"
                value={editingCar.licensePlate}
                onChange={handleEditChange}
              />

              <input
                name="brand"
                placeholder="Brand"
                value={editingCar.brand}
                onChange={handleEditChange}
              />

              <input
                name="model"
                placeholder="Model"
                value={editingCar.model}
                onChange={handleEditChange}
              />
              <label>Mileage:</label>
              <input
                name="distanceKm"
                type="number"
                placeholder="Mileage"
                value={editingCar.distanceKm}
                onChange={handleEditChange}
              />
              <label>Daily price:</label>
              <input
                name="dailyPrice"
                type="number"
                placeholder="Daily price"
                value={editingCar.dailyPrice}
                onChange={handleEditChange}
              />

              <select
                name="status"
                value={editingCar.status}
                onChange={handleEditChange}
              >
                <option value={0}>Available</option>
                <option value={1}>Unavailable</option>
                <option value={2}>Rented</option>
              </select>

              <label>Change image:</label>

              {editingCar.previewImageUrl && (
                <img
                  className="edit-image-preview"
                  src={editingCar.previewImageUrl}
                  alt="Car preview"
                />
              )}

              <input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
              />

              <label>Unavailable from:</label>
              <input
                name="unavailableFrom"
                type="date"
                value={editingCar.unavailableFrom}
                onChange={handleEditChange}
              />

              <label>Unavailable to:</label>
              <input
                name="unavailableTo"
                type="date"
                value={editingCar.unavailableTo}
                onChange={handleEditChange}
              />

              <select
                name="unavailableReason"
                value={editingCar.unavailableReason}
                onChange={handleEditChange}
              >
                <option value="">No reason</option>
                <option value={0}>Service</option>
                <option value={1}>Damaged</option>
                <option value={2}>Admin hold</option>
              </select>

              <textarea
                name="unavailableNote"
                placeholder="Note"
                value={editingCar.unavailableNote}
                onChange={handleEditChange}
              />

              <div className="modal-actions">
                <button type="submit" className="save-btn">
                  Save
                </button>

                <button
                  type="button"
                  className="cancel-btn"
                  onClick={() => setEditingCar(null)}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="admin-cars-list">
        {cars.map((car) => (
          <div key={car.id} className="admin-car-card">
            <img
              className="admin-car-image"
              src={car.imageUrl || "https://via.placeholder.com/300x180"}
              alt={`${car.brand} ${car.model}`}
            />

            <div className="admin-car-info">
              <h2>
                {car.brand} {car.model}
              </h2>

              <p>
                <strong>License plate:</strong> {car.licensePlate}
              </p>

              <p>
                <strong>Mileage:</strong> {car.distanceKm} km
              </p>

              <p>
                <strong>Daily price:</strong> {car.dailyPrice} Ft
              </p>

              <p>
                <strong>Status:</strong> {getStatusText(car.status)}
              </p>

              <p>
                <strong>Unavailable period:</strong>{" "}
                {car.unavailableFrom && car.unavailableTo
                  ? `${new Date(car.unavailableFrom).toLocaleDateString()} - ${new Date(
                      car.unavailableTo
                    ).toLocaleDateString()}`
                  : "Not set"}
              </p>

              <p>
                <strong>Note:</strong> {car.unavailableNote || "-"}
              </p>

              <div className="admin-car-actions">
                <button className="edit-btn" onClick={() => startEdit(car)}>
                  Edit
                </button>

                <button className="delete-btn" onClick={() => deleteCar(car.id)}>
                  Delete
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}