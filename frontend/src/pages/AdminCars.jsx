import React, { useEffect, useState } from "react";
import "../styles/AdminCars.css";

export default function AdminCars() {
  const [cars, setCars] = useState([]);
  const [editingCar, setEditingCar] = useState(null);
  const token = localStorage.getItem("token");

  const loadCars = async () => {
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
  };

  useEffect(() => {
    loadCars();
  }, []);

  const deleteCar = async (id) => {
    const confirmed = window.confirm("Biztosan törölni szeretnéd ezt az autót?");
    if (!confirmed) return;

    const res = await fetch(`https://localhost:7077/api/cars/${id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      alert("Törlés sikertelen!");
      return;
    }

    alert("Autó törölve!");
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
    });
  };

  const handleEditChange = (e) => {
    const { name, value } = e.target;

    setEditingCar((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const saveEdit = async (e) => {
    e.preventDefault();

    const body = {
      id: editingCar.id,
      licensePlate: editingCar.licensePlate,
      brand: editingCar.brand,
      model: editingCar.model,
      distanceKm: Number(editingCar.distanceKm),
      dailyPrice: Number(editingCar.dailyPrice),
      status: Number(editingCar.status),
      unavailableFrom: editingCar.unavailableFrom || null,
      unavailableTo: editingCar.unavailableTo || null,
      unavailableReason:
        editingCar.unavailableReason === ""
          ? null
          : Number(editingCar.unavailableReason),
      unavailableNote: editingCar.unavailableNote || null,
    };

    const res = await fetch(`https://localhost:7077/api/cars/${editingCar.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Update error:", res.status, text);
      alert("Módosítás sikertelen!");
      return;
    }

    alert("Autó módosítva!");
    setEditingCar(null);
    loadCars();
  };

  const getStatusText = (status) => {
    switch (status) {
      case 0:
        return "Elérhető";
      case 1:
        return "Nem elérhető";
      case 2:
        return "Kölcsönözve";
      default:
        return "Ismeretlen";
    }
  };

  return (
    <div className="admin-cars-page">
      <h1>Admin Cars</h1>
      <p>Autók megtekintése, módosítása, törlése és státusz kezelése.</p>

      {editingCar && (
        <div className="edit-modal-overlay">
          <div className="edit-modal">
            <h2>Autó módosítása</h2>

            <form onSubmit={saveEdit}>
              <input
                name="licensePlate"
                placeholder="Rendszám"
                value={editingCar.licensePlate}
                onChange={handleEditChange}
              />

              <input
                name="brand"
                placeholder="Márka"
                value={editingCar.brand}
                onChange={handleEditChange}
              />

              <input
                name="model"
                placeholder="Típus"
                value={editingCar.model}
                onChange={handleEditChange}
              />

              <input
                name="distanceKm"
                type="number"
                placeholder="Kilométeróra állás"
                value={editingCar.distanceKm}
                onChange={handleEditChange}
              />

              <input
                name="dailyPrice"
                type="number"
                placeholder="Napi ár"
                value={editingCar.dailyPrice}
                onChange={handleEditChange}
              />

              <select
                name="status"
                value={editingCar.status}
                onChange={handleEditChange}
              >
                <option value={0}>Elérhető</option>
                <option value={1}>Nem elérhető</option>
                <option value={2}>Kölcsönözve</option>
              </select>

              <label>Nem elérhető ettől:</label>
              <input
                name="unavailableFrom"
                type="date"
                value={editingCar.unavailableFrom}
                onChange={handleEditChange}
              />

              <label>Nem elérhető eddig:</label>
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
                <option value="">Nincs indok</option>
                <option value={0}>Szerviz</option>
                <option value={1}>Sérült</option>
                <option value={2}>Admin tiltás</option>
              </select>

              <textarea
                name="unavailableNote"
                placeholder="Megjegyzés"
                value={editingCar.unavailableNote}
                onChange={handleEditChange}
              />

              <div className="modal-actions">
                <button type="submit" className="save-btn">
                  Mentés
                </button>

                <button
                  type="button"
                  className="cancel-btn"
                  onClick={() => setEditingCar(null)}
                >
                  Mégse
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
                <strong>Rendszám:</strong> {car.licensePlate}
              </p>

              <p>
                <strong>Kilométeróra:</strong> {car.distanceKm} km
              </p>

              <p>
                <strong>Napi ár:</strong> {car.dailyPrice} Ft
              </p>

              <p>
                <strong>Státusz:</strong> {getStatusText(car.status)}
              </p>

              <p>
                <strong>Nem elérhető:</strong>{" "}
                {car.unavailableFrom && car.unavailableTo
                  ? `${new Date(car.unavailableFrom).toLocaleDateString()} - ${new Date(
                      car.unavailableTo
                    ).toLocaleDateString()}`
                  : "Nincs megadva"}
              </p>

              <p>
                <strong>Megjegyzés:</strong> {car.unavailableNote || "-"}
              </p>

              <div className="admin-car-actions">
                <button className="edit-btn" onClick={() => startEdit(car)}>
                  Módosítás
                </button>

                <button className="delete-btn" onClick={() => deleteCar(car.id)}>
                  Törlés
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}