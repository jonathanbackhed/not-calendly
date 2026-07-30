import api from "../api";

export const getEventTypes = (userSlug: string) => api.get(`/api/public/${userSlug}`).then((res) => res.data);
