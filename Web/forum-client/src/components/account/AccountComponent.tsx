import {FC, useState} from "react";
import {Button, Checkbox, FormControl, FormControlLabel, FormGroup, TextField} from "@mui/material";
import {registerApi} from "../ApiService";

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
    isSubscribedForMailing: false
}

const AccountComponent: FC<{}> = () => {
    const [userRequest, setUserRequest] = useState<RegisterRequest>(emptyRequest);
    function onFieldChange(field: string, value: string) {
        setUserRequest({...userRequest, [field]: value} as RegisterRequest);
    }

    function onClickHandler(fields: any) {
        registerApi(userRequest)
            .then(r => setUserRequest(emptyRequest));
    }

    return (<div>
        <div style={{
            display: 'flex',
            justifyContent: 'space-around'
        }}>
            <FormControl sx={{padding: '10px'}}>
                <TextField label="Username"
                           variant="filled"
                           required
                           value={userRequest?.username}
                           onChange={(event) => onFieldChange("username", event.target.value)}>
                </TextField>
                <TextField label="Password"
                           variant="filled"
                           required
                           value={userRequest?.password}
                           onChange={(event) => onFieldChange("password", event.target.value)}>
                </TextField>
                <TextField label="ConfirmPassword"
                           variant="filled"
                           required
                           value={userRequest?.confirmPassword}
                           onChange={(event) => onFieldChange("confirmPassword", event.target.value)}>
                </TextField>
                <TextField label="Email"
                           variant="filled"
                           required
                           value={userRequest?.email}
                           onChange={(event) => onFieldChange("email", event.target.value)}>
                </TextField>
                <FormControlLabel control={<Checkbox value={userRequest.isSubscribedForMailing}
                                                     onChange={(event) => onFieldChange("isSubscribedForMailing", event.target.value)}
                                                     defaultChecked/>} label="Sub to mailing"/>
                <Button variant="contained" onClick={onClickHandler}>Register</Button>
            </FormControl>
        </div>
    </div>)
}


export default AccountComponent;