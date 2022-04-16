import axios from "axios";

export function registerApi(body: any) {
    return axios.post("http://localhost:4900/auth/Register", body)
}

export function loginApi(body: any) {
    return axios.post("http://localhost:4900/auth/Login", body)
}

export function getQuestions()
{
    return axios.get("http://localhost:4900/forum/questions/")
}