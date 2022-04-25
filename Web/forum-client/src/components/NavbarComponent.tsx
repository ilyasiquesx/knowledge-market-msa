import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import {FC, useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {clearUser, getUser, isAuthenticated} from "./UserService";
import {Badge} from "@mui/material";
import NotificationsIcon from '@mui/icons-material/Notifications';
import {trackPromise} from "react-promise-tracker";
import {getNotifications} from "./ApiService";

interface Notification {
    isRead: boolean,
    content: Content
}

interface Content {
    message: string,
    questionId: number,
    questionTitle: string
}

const NavbarComponent: FC = () => {
    const navigate = useNavigate();
    const [notifications, setNotification] = useState<Notification[]>([]);

    useEffect(() => {
        if (isAuthenticated()) {
            trackPromise(getNotifications(), 'fetch-service').then(r => setNotification(r.data))
        }
    }, [])

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

    function toNotifications() {
        navigate('/notifications');
    }

    return (
        <Box sx={{flexGrow: 1}}>
            <AppBar position="static">
                <Toolbar>
                    <Box sx={{flexGrow: 1}}>
                        <Button color="inherit" onClick={() => window.location.assign("/")}>
                            Knowledge market
                        </Button>
                    </Box>

                    {isAuthenticated()
                        ?
                        <Box sx={{
                            display: 'flex',
                            alignItems: 'center'
                        }}>
                            <Typography sx={{marginX: '5px'}}>Welcome, {getUser()?.username ?? "guest"}</Typography>
                            <Button color="inherit" onClick={toNotifications}>
                                {
                                    notifications?.some(n => !n.isRead)
                                        ? <Badge color="error" variant="dot" sx={{marginX: '5px'}}>
                                            <NotificationsIcon/>
                                        </Badge>
                                        : <Badge color="error" sx={{marginX: '5px'}}>
                                            <NotificationsIcon/>
                                        </Badge>
                                }
                            </Button>
                            <Button color="inherit" onClick={onLogout} sx={{marginX: '5px'}}>Logout</Button>
                        </Box>
                        :
                        <Box>
                            <Button color="inherit" onClick={onLoginClickHandler} sx={{marginX: '5px'}}>Login</Button>
                        </Box>
                    }
                </Toolbar>
            </AppBar>
        </Box>)
}

export default NavbarComponent;