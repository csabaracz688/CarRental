import React, { createContext, useContext, useState, useCallback } from "react";
import "../styles/Confirm.css";

const ConfirmContext = createContext(null);

export function ConfirmProvider({ children }) {
  const [queue, setQueue] = useState([]);

  const confirm = useCallback((message, { title = "Confirm" } = {}) => {
    return new Promise((resolve) => {
      const id = Date.now() + Math.random();
      const onResult = (val) => {
        setQueue((q) => q.filter((i) => i.id !== id));
        resolve(val);
      };

      setQueue((q) => [...q, { id, title, message, onResult }]);
    });
  }, []);

  return (
    <ConfirmContext.Provider value={confirm}>
      {children}
      {queue.map((q) => (
        <div key={q.id} className="confirm-overlay">
          <div className="confirm-modal">
            <h3>{q.title}</h3>
            <p>{q.message}</p>
            <div className="confirm-actions">
              <button className="btn btn-secondary" onClick={() => q.onResult(false)}>
                No
              </button>
              <button className="btn btn-primary" onClick={() => q.onResult(true)}>
                Yes
              </button>
            </div>
          </div>
        </div>
      ))}
    </ConfirmContext.Provider>
  );
}

export function useConfirm() {
  const ctx = useContext(ConfirmContext);
  if (!ctx) throw new Error("useConfirm must be used within ConfirmProvider");
  return ctx;
}

export default ConfirmProvider;
