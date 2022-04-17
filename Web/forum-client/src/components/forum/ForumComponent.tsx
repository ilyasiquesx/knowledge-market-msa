import {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import {Grid, List, ListItem, Pagination} from "@mui/material";
import {Link as RouterLink} from "react-router-dom"
import {getQuestions} from "../ApiService";
import Button from "@mui/material/Button";

interface Question {
    title: string,
    author: User,
    id: number,
    createdAt: string
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
        pageSize: 2,
        page: 1
    })

    useEffect(() => {
        questionsUpdate(paginationRequest)
    }, [])

    function questionsUpdate(pagination: any) {
        getQuestions({
            page: pagination.page,
            pageSize: pagination.pageSize
        }).then(r => {
            console.log(r.data);
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
                border: '1px solid #8472fc',
                borderRadius: '10px',
                minWidth: '450px'
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
                </Box>

            </ListItem>
        )
    }

    return (
        <Box>
            <Box sx={{
                display: 'flex',
                alignItems: 'center',
                flexDirection: 'column'
            }}>
                <Typography textAlign="center" sx={{
                    margin: '5px',
                    padding: '10px'
                }}>Questions list</Typography>
                <Button variant="contained" sx={{margin: '5px'}}>Ask a question</Button>
            </Box>
                <List sx={{
                    display: 'flex',
                    gap: '10px',
                    flexDirection: 'column',
                    alignItems: 'space-between'
                }}>
                    {questions?.map(BuildTopic)}
                </List>
                <Box sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between'
                }}>
                    <Pagination count={pagesCount} onChange={(e, v) => questionsUpdate({
                        page: v,
                        pageSize: paginationRequest.pageSize
                    })} variant="outlined" color="primary"/>
                </Box>
        </Box>)
}

export default ForumComponent;