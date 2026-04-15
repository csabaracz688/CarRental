import { useState } from "react";

function AdminAddCar() {
  const [car, setCar] = useState({
    brand: "",
    model: "",
    price: "",
  });

  const handleChange = (e) => {
    setCar({ ...car, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Új autó:", car);
  };

  return (
    <div>
      <h1>Autó hozzáadása</h1>

      <form onSubmit={handleSubmit}>
        <input
          type="text"
          name="brand"
          placeholder="Márka"
          value={car.brand}
          onChange={handleChange}
        />
        <br />

        <input
          type="text"
          name="model"
          placeholder="Model"
          value={car.model}
          onChange={handleChange}
        />
        <br />

        <input
          type="number"
          name="price"
          placeholder="Ár / nap"
          value={car.price}
          onChange={handleChange}
        />
        <br />

        <button type="submit">Mentés</button>
      </form>
    </div>
  );
}

export default AdminAddCar;