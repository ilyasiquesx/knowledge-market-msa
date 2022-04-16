import {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import {useParams} from "react-router-dom"


const QuestionComponent: FC<{}> = () => {
    const id = useParams();
    console.log(id);
    return (<Box sx={{
        display: 'flex',
        justifyContent: 'center',
        margin: '10px'
    }}>
    </Box>)
}

export default QuestionComponent;