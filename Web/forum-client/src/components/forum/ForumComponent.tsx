import React, {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import {Grid, List, ListItem, Pagination} from "@mui/material";
import {BrowserRouter as Router, Link as RouterLink, useNavigate} from "react-router-dom"
import {getQuestions} from "../ApiService";
import Button from "@mui/material/Button";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";
import NavbarComponent from "../NavbarComponent";

interface Question {
    title: string,
    author: User,
    id: number,
    createdAt: string,
    answersCount : number
}

interface User {
    username: string,
    id: string
}

interface Pagination {
    pageSize: number,
    page: number
}


const ForumComponent: FC<{}> = () => {

    const [questions, setQuestions] = useState<Question[]>([]);
    const [pagesCount, setPagesCount] = useState<number>(0);
    const [paginationRequest, setPaginationRequest] = useState<Pagination>({
        pageSize: 6,
        page: 1
    })

    const navigate = useNavigate();

    useEffect(() => {
        questionsUpdate(paginationRequest)
    }, [])

    function questionsUpdate(pagination: any) {
        trackPromise(getQuestions({
            page: pagination.page,
            pageSize: pagination.pageSize
        }), 'fetch-service').then(r => {
            console.log(r?.data);
            setQuestions(r.data?.questions);
            setPagesCount(r.data?.pageCount);
        })
    }

    function BuildTopic(question: Question) {
        return (
            <ListItem disablePadding key={question?.id} sx={{
                padding: '10px',
                display: 'flex',
                gap: '10px',
                border: '2px solid #a9c9fc',
                borderRadius: '10px',
                minWidth: '320px',
                backgroundColor: '#e6eefc'
            }}>

                <RouterLink to={`question/${question?.id}`}
                            style={{
                                flexGrow: '1',
                                textDecoration: 'none'
                            }}>
                    {question?.title}
                </RouterLink>
                <Box>
                    <Typography>Created by: {question?.author.username}</Typography>
                    <Typography>Created at: {question?.createdAt}</Typography>
                    <Typography align="right">Answers: {question?.answersCount}</Typography>
                </Box>

            </ListItem>
        )
    }

    return (
        <Box>
            <Box sx={{
                display: 'flex',
                alignItems: 'center',
                flexDirection: 'column',
                marginBottom: '10px',
            }}>
                <Typography variant="h4" textAlign="center" sx={{
                    margin: '5px',
                    padding: '10px'
                }}>Questions list</Typography>
                <Button variant="contained" sx={{margin: '5px'}} onClick={() => {
                    navigate("/question/create");
                }}>Ask a question</Button>
            </Box>
            <Box sx={{
                display: 'flex',
                justifyContent: 'center'
            }}>
                <Pagination count={pagesCount} onChange={(e, v) => questionsUpdate({
                    page: v,
                    pageSize: paginationRequest.pageSize
                })} variant="outlined" color="primary"/>
            </Box>
            <ProgressComponent/>
            {questions?.length < 1 && <Typography mt="10px" variant="h4">There are no questions yes. You can ask one.</Typography>}
            <List sx={{
                display: 'flex',
                gap: '10px',
                flexDirection: 'column',
                alignItems: 'space-between'
            }}>
                {questions?.map(BuildTopic)}
            </List>
        </Box>)
}

export default ForumComponent;