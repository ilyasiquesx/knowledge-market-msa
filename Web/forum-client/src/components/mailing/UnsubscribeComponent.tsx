import {FC} from "react";
import {Box, Button} from "@mui/material";
import {useNavigate, useParams} from "react-router-dom";
import {putMailing} from "../ApiService";
import {toast} from "react-toastify";

const UnsubscribeComponent: FC = () => {
    const {userId} = useParams();
    const navigate = useNavigate();

    function updateMailing() {
        putMailing(userId as string).then(() => {
            toast("Unsubed successful");
            navigate("/");
        })
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            margin: '10px'
        }}>
            <Button variant="contained" onClick={updateMailing}>Unsubscribe from email notifications</Button>
        </Box>)
}

export default UnsubscribeComponent;