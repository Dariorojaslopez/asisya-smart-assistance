import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:8080",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Auth: solo POST /auth/login
export const login = async (username, password) => {
  const response = await api.post("/auth/login", { username, password });
  return response.data;
};

// Catálogo de tipos de asistencia (GET /catalogos/tipos-asistencia devuelve un objeto { "1": "Grúa", ... })
export const getAssistanceTypes = async () => {
  const response = await api.get("/catalogos/tipos-asistencia");
  return response.data;
};

// Optimize (proveedores ordenados con ETA)
export const optimizeProviders = async (latitude, longitude, assistanceType) => {
  const response = await api.post("/optimize", {
    latitude,
    longitude,
    assistanceType,
  });
  return response.data;
};

// Users CRUD
export const getUsers = async () => {
  const response = await api.get("/users");
  return response.data;
};

export const getUser = async (id) => {
  const response = await api.get(`/users/${id}`);
  return response.data;
};

export const createUser = async (data) => {
  const response = await api.post("/users", data);
  return response.data;
};

export const updateUser = async (id, data) => {
  const response = await api.put(`/users/${id}`, data);
  return response.data;
};

export const deleteUser = async (id) => {
  await api.delete(`/users/${id}`);
};
