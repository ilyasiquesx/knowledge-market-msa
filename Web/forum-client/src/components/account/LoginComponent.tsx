import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import {FC, useState} from "react";
import {setUser, User} from "../../services/UserService";
import {FormControl, TextField} from "@mui/material";
import {loginApi} from "../../services/ApiService";
import Typography from "@mui/material/Typography";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";

interface LoginRequest {
    username: string,
    password: string,
}

const emptyLoginRequest = {
    username: '',
    password: '',
}

const LoginComponent: FC = () => {
    const [loginRequest, setLoginRequest] = useState<LoginRequest>(emptyLoginRequest);

    function onLoginClickHandler() {
        trackPromise(loginApi(loginRequest), 'fetch-service')
            .then(r => {
                if (r?.data != null) {
                    const user = {
                        id: r?.data?.id,
                        username: r?.data?.username,
                        accessToken: r?.data?.accessToken
                    } as User;
                    setUser(user)
                    window.location.assign("/");
                }
            });
    }

    function onFieldChange(field: string, value: string) {
        setLoginRequest({...loginRequest, [field]: value} as LoginRequest);
    }

    return (
        <Box>
            <FormControl sx={{padding: '10px'}}>
                <Typography align="center">Sign in form</Typography>
                <TextField label="Username"
                           variant="filled"
                           required
                           value={loginRequest?.username}
                           onChange={(event) => onFieldChange("username", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <TextField label="Password"
                           variant="filled"
                           required
                           value={loginRequest?.password}
                           onChange={(event) => onFieldChange("password", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <Button variant="contained" onClick={onLoginClickHandler}>Sign in</Button>
                <ProgressComponent/>
            </FormControl>
        </Box>)
}

export default LoginComponent;