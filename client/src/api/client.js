import axios from "axios";

const http = axios.create({ baseURL: "/api" });

export const peopleApi = {
  getAll: () => http.get("/people").then((r) => r.data),
  getTodayAndUpcoming: () => http.get("/people/today-upcoming").then((r) => r.data),
  getById: (id) => http.get(`/people/${id}`).then((r) => r.data),
  create: (dto) => http.post("/people", dto).then((r) => r.data),
  update: (id, dto) => http.put(`/people/${id}`, dto),
  remove: (id) => http.delete(`/people/${id}`),
  setPhoto: (id, file) => {
    const form = new FormData();
    form.append("photo", file);
    return http
      .post(`/people/${id}/photo`, form, {
        headers: { "Content-Type": "multipart/form-data" },
      })
      .then((r) => r.data);
  },
  removePhoto: (id) => http.delete(`/people/${id}/photo`).then((r) => r.data),
};

export const notificationsApi = {
  runNow: () => http.post("/notifications/run-now").then((r) => r.data),
};

export default http;
