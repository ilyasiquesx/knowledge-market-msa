import axios from "axios";
import {clearUser, getUser} from "./UserService";
import {toast} from "react-toastify";

const instance = axios.create();
const baseUrl = process.env.REACT_APP_GATEWAY_URL;

instance.interceptors.response.use((r => {
    return r;
}), er => {
    if (er.response.status == 401) {
        clearUser();
        window.location.assign("/auth");
    }

    toast.error(er?.response?.data?.message
        || er?.response?.data?.title
        || er?.message);

    return er;
})

instance.interceptors.request.use((r) => {
    r.headers = {
        Authorization: `Bearer ${getUser()?.accessToken}`
    }

    return r;
});

export function registerApi(body: any) {
    return instance.post(`${baseUrl}/auth/Register`, body)
}

export function loginApi(body: any) {
    return instance.post(`${baseUrl}/auth/Login`, body)
}

export function getQuestions(request: any) {
    return instance.get(`${baseUrl}/forum/questions/`, {
        params: {
            pageSize: request.pageSize,
            page: request.page
        }
    })
}

export function getQuestionById(id: string) {
    return instance.get(`${baseUrl}/forum/questions/${id}`);
}

export function postAnswer(body: any) {
    return instance.post(`${baseUrl}/forum/Answers`, body)
}

export function postQuestion(body: any) {
    return instance.post(`${baseUrl}/forum/Questions`, body)
}

export function putQuestion(id: string, body: any) {
    return instance.put(`${baseUrl}/forum/Questions/${id}`, body)
}

export function deleteQuestion(id: string) {
    return instance.delete(`${baseUrl}/forum/Questions/${id}`)
}

export function getNotifications() {
    return instance.get(`${baseUrl}/notifications`)
}

export function putNotifications() {
    return instance.put(`${baseUrl}/notifications`)
}

export function putMailing(userId: string) {
    return instance.put(`${baseUrl}/mailing/${userId}`)
}

export function putAnswer(id: string, body: any) {
    return instance.put(`${baseUrl}/forum/Answers/${id}`, body)
}

export function deleteAnswer(id: string) {
    return instance.delete(`${baseUrl}/forum/Answers/${id}`)
}

export function getAnswerById(id: string) {
    return instance.get(`${baseUrl}/forum/Answers/${id}`)
}