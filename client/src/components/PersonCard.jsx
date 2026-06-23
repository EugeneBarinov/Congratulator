import { Link } from "react-router-dom";

function initials(person) {
  return `${person.firstName?.[0] ?? ""}${person.lastName?.[0] ?? ""}`.toUpperCase();
}

function formatDate(dateOnly) {
  const [year, month, day] = dateOnly.split("-").map(Number);
  return new Date(year, month - 1, day).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "long",
  });
}

export default function PersonCard({ person, onDelete }) {
  const rowClass = person.isToday ? "is-today" : person.isUpcoming ? "is-soon" : "";

  let badgeClass = "later";
  let badgeText = `через ${person.daysUntilNextBirthday} дн.`;
  if (person.isToday) {
    badgeClass = "today";
    badgeText = "Сегодня 🎂";
  } else if (person.isUpcoming) {
    badgeClass = "soon";
  }

  return (
    <li className={`ledger-row ${rowClass}`}>
      {person.photoUrl ? (
        <img className="ledger-photo" src={person.photoUrl} alt={`Фото ${person.firstName}`} />
      ) : (
        <div className="ledger-photo-placeholder" aria-hidden="true">
          {initials(person)}
        </div>
      )}

      <div className="ledger-main">
        <div className="ledger-name">
          {person.lastName} {person.firstName}
        </div>
        <div className="ledger-meta">
          {person.isToday ? `Исполняется ${person.age}` : `Исполнится ${person.age}`}
          {person.relation && (
            <>
              <span className="dot">·</span>
              {person.relation}
            </>
          )}
        </div>
      </div>

      <div className="ledger-date">{formatDate(person.nextOccurrence)}</div>

      <span className={`ledger-badge ${badgeClass}`}>{badgeText}</span>

      <div className="ledger-actions">
        <Link to={`/edit/${person.id}`} className="btn btn-ghost btn-icon" aria-label="Редактировать" title="Редактировать">
          ✎
        </Link>
        <button
          type="button"
          className="btn btn-ghost btn-icon"
          aria-label="Удалить"
          title="Удалить"
          onClick={() => onDelete(person)}
        >
          ✕
        </button>
      </div>
    </li>
  );
}
