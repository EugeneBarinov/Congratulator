import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { peopleApi } from "../api/client.js";

const emptyForm = { firstName: "", lastName: "", birthDate: "", relation: "", notes: "" };

export default function PersonFormPage({ mode }) {
  const { id } = useParams();
  const navigate = useNavigate();

  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState({});
  const [existingPhotoUrl, setExistingPhotoUrl] = useState(null);
  const [photoFile, setPhotoFile] = useState(null);
  const [photoRemoved, setPhotoRemoved] = useState(false);
  const [saving, setSaving] = useState(false);
  const [loadError, setLoadError] = useState(null);
  const fileInputRef = useRef(null);

  useEffect(() => {
    if (mode !== "edit") return;
    peopleApi
      .getById(id)
      .then((p) => {
        setForm({
          firstName: p.firstName,
          lastName: p.lastName,
          birthDate: p.birthDate,
          relation: p.relation ?? "",
          notes: p.notes ?? "",
        });
        setExistingPhotoUrl(p.photoUrl);
      })
      .catch(() => setLoadError("Не удалось загрузить запись."));
  }, [mode, id]);

  const previewUrl = photoFile ? URL.createObjectURL(photoFile) : photoRemoved ? null : existingPhotoUrl;

  const setField = (field) => (e) => setForm((f) => ({ ...f, [field]: e.target.value }));

  const validate = () => {
    const next = {};
    if (!form.firstName.trim()) next.firstName = "Укажите имя";
    if (!form.lastName.trim()) next.lastName = "Укажите фамилию";
    if (!form.birthDate) next.birthDate = "Укажите дату рождения";
    else if (form.birthDate > new Date().toISOString().slice(0, 10)) {
      next.birthDate = "Дата рождения не может быть в будущем";
    }
    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handlePhotoChange = (e) => {
    const file = e.target.files?.[0];
    if (file) {
      setPhotoFile(file);
      setPhotoRemoved(false);
    }
  };

  const handleRemovePhoto = () => {
    setPhotoFile(null);
    setPhotoRemoved(true);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setSaving(true);
    try {
      const dto = {
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        birthDate: form.birthDate,
        relation: form.relation.trim() || null,
        notes: form.notes.trim() || null,
      };

      let personId = id;
      if (mode === "create") {
        const created = await peopleApi.create(dto);
        personId = created.id;
      } else {
        await peopleApi.update(id, dto);
      }

      if (photoFile) {
        await peopleApi.setPhoto(personId, photoFile);
      } else if (photoRemoved && mode === "edit") {
        await peopleApi.removePhoto(personId);
      }

      navigate("/all");
    } catch (err) {
      const serverMessage = err?.response?.data?.message ?? "Не удалось сохранить запись. Проверьте данные и попробуйте снова.";
      setErrors((prev) => ({ ...prev, form: serverMessage }));
    } finally {
      setSaving(false);
    }
  };

  if (loadError) {
    return (
      <div className="page-header">
        <h1>Запись не найдена</h1>
        <p>{loadError}</p>
      </div>
    );
  }

  return (
    <>
      <div className="page-header">
        <h1>{mode === "create" ? "Новая запись" : "Редактирование записи"}</h1>
        <p>Имя, дата рождения и (по желанию) фотография именинника.</p>
      </div>

      <form className="form-card" onSubmit={handleSubmit}>
        <div className="photo-uploader">
          {previewUrl ? (
            <img className="photo-preview" src={previewUrl} alt="Предпросмотр фото" />
          ) : (
            <div className="photo-preview-placeholder" aria-hidden="true" />
          )}

          <div>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/gif,image/webp"
              onChange={handlePhotoChange}
              style={{ display: "none" }}
              id="photo-input"
            />
            <label htmlFor="photo-input" className="btn btn-ghost" style={{ cursor: "pointer" }}>
              {previewUrl ? "Заменить фото" : "Загрузить фото"}
            </label>
            {previewUrl && (
              <button type="button" className="btn btn-ghost" onClick={handleRemovePhoto} style={{ marginLeft: 8 }}>
                Удалить фото
              </button>
            )}
          </div>
        </div>

        <div className="form-grid">
          <div className="form-field">
            <label htmlFor="firstName">Имя</label>
            <input id="firstName" value={form.firstName} onChange={setField("firstName")} />
            {errors.firstName && <span className="field-error">{errors.firstName}</span>}
          </div>

          <div className="form-field">
            <label htmlFor="lastName">Фамилия</label>
            <input id="lastName" value={form.lastName} onChange={setField("lastName")} />
            {errors.lastName && <span className="field-error">{errors.lastName}</span>}
          </div>

          <div className="form-field">
            <label htmlFor="birthDate">Дата рождения</label>
            <input id="birthDate" type="date" value={form.birthDate} onChange={setField("birthDate")} />
            {errors.birthDate && <span className="field-error">{errors.birthDate}</span>}
          </div>

          <div className="form-field">
            <label htmlFor="relation">Кто это (необязательно)</label>
            <input id="relation" placeholder="например, коллега, друг" value={form.relation} onChange={setField("relation")} />
          </div>

          <div className="form-field full">
            <label htmlFor="notes">Заметки (необязательно)</label>
            <textarea id="notes" rows={3} value={form.notes} onChange={setField("notes")} />
          </div>
        </div>

        {errors.form && <p className="field-error">{errors.form}</p>}

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={saving}>
            {saving ? "Сохранение…" : "Сохранить"}
          </button>
          <button type="button" className="btn btn-ghost" onClick={() => navigate(-1)}>
            Отмена
          </button>
        </div>
      </form>
    </>
  );
}
