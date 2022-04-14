import axios from "axios";

export  function registerApi(body : any)
{
    return axios.post("https://localhost:4900/auth/Register", body)
}