import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import {FC, useEffect, useState} from "react";
import {useNavigate, Link} from "react-router-dom";
import {clearUser, getUser, isAuthenticated, User} from "./UserService";

const NavbarComponent: FC<{}> = () => {
    const navigate = useNavigate();
    function onLoginClickHandler() {
        if (isAuthenticated()) {
            navigate('/')
        } else {
            navigate('/auth');
        }
    }

    function onLogout() {
        clearUser();
        navigate('/');
    }
    console.log("Navbar render");
    return (
        <Box sx={{flexGrow: 1}}>
            <AppBar position="static">
                <Toolbar>
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{mr: 2}}>
                    </IconButton>

                    <Box sx={{flexGrow: 1}}>
                        <Button color="inherit" onClick={() => navigate('/')}>
                            Knowledge market
                        </Button>
                    </Box>

                    {isAuthenticated()
                        ?
                        <Box sx={{
                            display: 'flex',
                            alignItems: 'center'
                        }}>
                            <Typography>{getUser()?.username ?? "Username"}</Typography>
                            <Button color="inherit" onClick={onLogout}>Logout</Button>
                        </Box>
                        :
                        <Box>
                            <Button color="inherit" onClick={onLoginClickHandler}>Login</Button>
                        </Box>
                    }
                </Toolbar>
            </AppBar>
        </Box>)
}

export default NavbarComponent;