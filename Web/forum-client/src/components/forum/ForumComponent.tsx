import React, {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import {Grid, Pagination} from "@mui/material";
import {Link as RouterLink, useNavigate} from "react-router-dom"
import {getQuestions} from "../ApiService";
import Button from "@mui/material/Button";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";

interface Question {
    title: string,
    author: User,
    id: number,
    createdAt: string,
    answersCount: number
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
    const [fetchError, setFetchError] = useState<boolean>(false);
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
            setQuestions(r.data?.questions);
            setPagesCount(r.data?.pageCount);
        }).catch(er => setFetchError(true));
    }

    function BuildTopic(question: Question) {
        return (
            <Grid item>
                <Grid container
                      direction="row"
                      key={question?.id}
                      alignItems="center"
                      sx={{
                          backgroundColor: '#e6eefc',
                      }}>
                    <Grid xl={2} sm={3} xs={4}>
                        <Typography align="center">Answers</Typography>
                        <Typography align="center">{question?.answersCount}</Typography>
                    </Grid>
                    <Grid xl={10} sm={9} xs={8}>
                        <RouterLink to={`question/${question?.id}`}
                                    style={{
                                        textDecoration: 'none'
                                    }}>
                            {question?.title}
                        </RouterLink>
                        <Box m="5px">
                            <Typography align="right">Created by: {question?.author.username}</Typography>
                            <Typography align="right">Created at: {question?.createdAt}</Typography>
                        </Box>
                    </Grid>
                </Grid>
            </Grid>
        )
    }

    return (
        <Grid sx={{
            width: '100%'
        }}>
            <Grid container alignItems="center" justifyContent="center">
                <Grid item>
                    <Typography variant="h4" textAlign="center" sx={{
                        margin: '5px',
                        padding: '10px'
                    }}>Questions list</Typography>
                </Grid>
                <Grid item>
                    <Box>
                        <Button variant="contained" sx={{margin: '5px'}} onClick={() => {
                            navigate("/question/create");
                        }}>Ask a question</Button>
                    </Box>
                </Grid>
            </Grid>
            <ProgressComponent/>
            {fetchError
                ? <Typography align="center" mt="10px" variant="h4">Something went wrong. Sorry.</Typography>
                : questions?.length < 1 &&
                <Typography align="center" mt="10px" variant="h4">There are no questions yes. You can ask
                    one.</Typography>}

            {questions?.length > 0 && <Grid container alignItems="center" justifyContent="center">
                <Pagination count={pagesCount} onChange={(e, v) => questionsUpdate({
                    page: v,
                    pageSize: paginationRequest.pageSize
                })} variant="outlined" color="primary"/>
            </Grid>}
            <Grid container
                  direction="column"
                  mt="10px"
                  spacing="10">

                {questions?.map(BuildTopic)}
            </Grid>
        </Grid>)
}

export default ForumComponent;