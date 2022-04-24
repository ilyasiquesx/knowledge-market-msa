import {FC, useEffect, useState} from "react";
import {Box, Button} from "@mui/material";
import LoginComponent from "./LoginComponent";
import RegisterComponent from "./RegisterComponent";
import {isAuthenticated} from "../UserService";


const AccountComponent: FC = () => {
    const [currentForm, setCurrentFrom] = useState(<LoginComponent/>);

    useEffect(() => {
        if (isAuthenticated()) {
            window.location.assign("/");
        }
    }, [])

    function setLogin() {
        setCurrentFrom(<LoginComponent/>)
    }

    function setRegister() {
        setCurrentFrom(<RegisterComponent/>)
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            margin: '10px'
        }}>
            <Box sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center'
            }}>
                <Box>
                    <Button variant="contained" onClick={setLogin} sx={{margin: '5px'}}>Sign in</Button>
                    <Button variant="contained" onClick={setRegister} sx={{margin: '5px'}}>Sign up</Button>
                </Box>
                {currentForm}
            </Box>
        </Box>)
}

export default AccountComponent;