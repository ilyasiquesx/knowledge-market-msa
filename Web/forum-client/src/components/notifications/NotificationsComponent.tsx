import {FC, useEffect, useState} from "react";
import {Box, Button, Grid, Typography} from "@mui/material";
import {isAuthenticated} from "../UserService";
import {trackPromise} from "react-promise-tracker";
import {getNotifications, putNotifications} from "../ApiService";

interface Notification {
    isRead: boolean,
    content: string
    createdAt: string
}

const NotificationsComponent: FC = () => {

    const [notifications, setNotification] = useState<Notification[]>([]);

    useEffect(() => {
        setNotifications();
    }, [])

    function renderNotification(item: Notification) {
        return (<Box sx={{
            borderTop: '1px solid black',
            backgroundColor: '#e6eefc',
            padding: '5px'
        }}>
            <Grid item><Typography
                dangerouslySetInnerHTML={{__html: item.content}}
                fontWeight={item?.isRead ? "regular" : "bold"}/></Grid>
            <Grid item>{item?.createdAt}</Grid>
        </Box>)
    }

    function setNotifications() {
        if (isAuthenticated()) {
            trackPromise(getNotifications(), 'fetch-service').then(r => setNotification(r.data))
        }
    }

    function onMarkAsReadClickHandler() {
        putNotifications().then(() => {
            setNotifications();
        })
    }

    return (
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
        }}>
            {notifications?.length > 0 ?
                <Box padding="10px">
                    <Box sx={{
                        textAlign: 'right',
                        paddingBottom: '10px'
                    }}>
                        <Button variant="contained" onClick={onMarkAsReadClickHandler}>Mark all as read</Button>
                    </Box>
                    {notifications?.map(renderNotification)}
                </Box>
                : <Typography variant="h4" align="center">There are no notifications for you</Typography>}
        </Box>)
}

export default NotificationsComponent;