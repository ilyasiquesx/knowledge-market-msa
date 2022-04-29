import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import {FC, useState} from "react";
import {Checkbox, FormControl, FormControlLabel, TextField} from "@mui/material";
import {registerApi} from "../../services/ApiService";
import Typography from "@mui/material/Typography";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";
import {toast} from "react-toastify";

interface RegisterRequest {
    username: string,
    password: string,
    confirmPassword: string,
    email: string,
    isSubscribedForMailing: boolean
}

const emptyRequest = {
    username: '',
    password: '',
    confirmPassword: '',
    email: '',
    isSubscribedForMailing: true
}

const RegisterComponent: FC = () => {
    const [userRequest, setUserRequest] = useState<RegisterRequest>(emptyRequest);

    function onFieldChange(field: string, value: any) {
        setUserRequest({...userRequest, [field]: value} as RegisterRequest);
    }

    function onRegisterClickHandler() {
        trackPromise(registerApi(userRequest), 'fetch-service')
            .then((r) => {
                if (r?.status === 200) {
                    setUserRequest(emptyRequest)
                    toast("Successful registration");
                }
            });
    }

    return (
        <Box>
            <FormControl sx={{padding: '10px'}}>
                <Typography align="center">Sign up form</Typography>
                <TextField label="Username"
                           variant="filled"
                           required
                           value={userRequest?.username}
                           onChange={(event) => onFieldChange("username", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <TextField label="Password"
                           variant="filled"
                           required
                           value={userRequest?.password}
                           onChange={(event) => onFieldChange("password", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <TextField label="Confirm password"
                           variant="filled"
                           required
                           value={userRequest?.confirmPassword}
                           onChange={(event) => onFieldChange("confirmPassword", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <TextField label="Email"
                           variant="filled"
                           required
                           value={userRequest?.email}
                           onChange={(event) => onFieldChange("email", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
                <FormControlLabel control={<Checkbox checked={userRequest.isSubscribedForMailing}
                                                     onClick={() => {
                                                         onFieldChange("isSubscribedForMailing", !userRequest.isSubscribedForMailing)
                                                     }}
                                                     defaultChecked/>} label="Sub to mailing"/>
                <Button variant="contained" onClick={onRegisterClickHandler}>Sign up</Button>
                <ProgressComponent/>
            </FormControl>
        </Box>)
}

export default RegisterComponent;