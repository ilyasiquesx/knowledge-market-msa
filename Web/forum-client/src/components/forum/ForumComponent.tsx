import {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import {Link, List, ListItem, ListItemButton, ListItemText, Pagination, PaginationItem} from "@mui/material";
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import {DataGrid} from "@mui/x-data-grid";
import {Link as RouterLink} from "react-router-dom"
import {Router} from "@mui/icons-material";
import {getQuestions} from "../ApiService";
import Button from "@mui/material/Button";

interface Question {
    title: string,
    createdBy: string,
    id: number,
}


interface Pagination {
    pageSize: number,
}

const ForumComponent: FC<{}> = () => {

    const [questions, setQuestions] = useState<Question[]>([]);
    useEffect(() => {
        getQuestions().then(r => {
            console.log(r.data);
            setQuestions(r.data);
        })
    }, [])

    function BuildTopic(question: Question) {
        return (
            <ListItem disablePadding key={question?.id}>
                <RouterLink to={`question/${question?.id}`}>{question?.title}</RouterLink>
            </ListItem>)
    }

    return (
        <Box>
            <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                flexDirection: 'column'
            }}>
                <Typography textAlign="center" sx={{margin: '5px'}}>Questions list</Typography>
                <Button variant="contained" sx={{margin: '5px'}}>Ask a question</Button>
            </Box>
            <Box sx={{
                display: 'flex',
                justifyContent: 'center'
            }}>
                <List>
                    {questions?.map(BuildTopic)}
                </List>
            </Box>
        </Box>)
}

export default ForumComponent;