import {FC, useEffect, useState} from "react";
import {Box, Button} from "@mui/material";
import LoginComponent from "./LoginComponent";
import RegisterComponent from "./RegisterComponent";
import {isAuthenticated} from "../UserService";
import {useNavigate} from "react-router-dom";


const AccountComponent: FC<{ /*onUserChange: (user: User) => void*/ }> = (/*{onUserChange}*/) => {
    const [currentForm, setCurrentFrom] = useState(<LoginComponent /*onUserChange={onUserChange}*//>);
    const navigate = useNavigate();
    useEffect(() => {
        if (isAuthenticated()) {
            navigate('/')
        }
    }, [])

    function setLogin() {
        setCurrentFrom(<LoginComponent /*onUserChange={onUserChange}*//>)
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