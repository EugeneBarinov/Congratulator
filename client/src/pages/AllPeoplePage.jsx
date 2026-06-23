import { useEffect, useMemo, useState } from "react";
import { peopleApi } from "../api/client.js";
import PersonCard from "../components/PersonCard.jsx";
import EmptyState from "../components/EmptyState.jsx";
import ConfirmDialog from "../components/ConfirmDialog.jsx";

export default function AllPeoplePage() {
  const [people, setPeople] = useState(null);
  const [error, setError] = useState(null);
  const [query, setQuery] = useState("");
  const [toDelete, setToDelete] = useState(null);

  const load = () => {
    setError(null);
    peopleApi
      .getAll()
      .then(setPeople)
      .catch(() => setError("Не удалось загрузить список. Проверьте, что API запущено."));
  };

  useEffect(load, []);

  const filtered = useMemo(() => {
    if (!people) return [];
    const q = query.trim().toLowerCase();
    if (!q) return people;
    return people.filter((p) =>
      [p.firstName, p.lastName, p.relation].filter(Boolean).some((field) => field.toLowerCase().includes(q))
    );
  }, [people, query]);

  const handleDelete = async () => {
    await peopleApi.remove(toDelete.id);
    setToDelete(null);
    load();
  };

  return (
    <>
      <div className="page-header">
        <h1>Все дни рождения</h1>
        <p>Полный список, отсортированный по близости даты.</p>
      </div>

      {people && people.length > 0 && (
        <div className="search-bar">
          <input
            type="search"
            placeholder="Поиск по имени, фамилии или статусу…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            aria-label="Поиск"
          />
        </div>
      )}

      {error && <EmptyState title="Что-то пошло не так" description={error} />}

      {!error && people === null && <p style={{ color: "var(--color-ink-soft)" }}>Загрузка…</p>}

      {!error && people !== null && people.length === 0 && (
        <EmptyState
          title="Список пуст"
          description="Добавьте первую запись, чтобы начать вести дни рождения друзей и коллег."
          actionLabel="Добавить запись"
          actionTo="/add"
        />
      )}

      {!error && people !== null && people.length > 0 && filtered.length === 0 && (
        <EmptyState title="Ничего не найдено" description="Попробуйте другой запрос." />
      )}

      {filtered.length > 0 && (
        <ul className="ledger">
          {filtered.map((person) => (
            <PersonCard key={person.id} person={person} onDelete={setToDelete} />
          ))}
        </ul>
      )}

      <ConfirmDialog
        open={Boolean(toDelete)}
        title="Удалить запись?"
        description={toDelete ? `${toDelete.lastName} ${toDelete.firstName} будет удалён из списка без возможности восстановления.` : ""}
        onConfirm={handleDelete}
        onCancel={() => setToDelete(null)}
      />
    </>
  );
}
