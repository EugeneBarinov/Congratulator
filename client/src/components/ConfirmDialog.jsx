export default function ConfirmDialog({ open, title, description, onConfirm, onCancel }) {
  if (!open) {
    return null;
  }

  return (
    <div
      role="dialog"
      aria-modal="true"
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(32, 38, 43, 0.35)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        zIndex: 100,
        padding: 20,
      }}
    >
      <div
        style={{
          background: "var(--color-surface)",
          borderRadius: "var(--radius)",
          boxShadow: "var(--shadow-card)",
          padding: 24,
          maxWidth: 360,
          width: "100%",
        }}
      >
        <h3 style={{ marginBottom: 8 }}>{title}</h3>
        <p style={{ color: "var(--color-ink-soft)", marginTop: 0 }}>{description}</p>
        <div className="form-actions" style={{ marginTop: 16, justifyContent: "flex-end" }}>
          <button type="button" className="btn btn-ghost" onClick={onCancel}>
            Отмена
          </button>
          <button type="button" className="btn btn-danger" onClick={onConfirm}>
            Удалить
          </button>
        </div>
      </div>
    </div>
  );
}
