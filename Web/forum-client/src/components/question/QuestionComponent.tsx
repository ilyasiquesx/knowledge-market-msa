import React, {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import {useNavigate, useParams} from "react-router-dom"
import Typography from "@mui/material/Typography";
import {Grid, Pagination, TextareaAutosize} from "@mui/material";
import {deleteQuestion, getQuestionById, postAnswer, putQuestion} from "../ApiService";
import Button from "@mui/material/Button";
import {isAuthenticated} from "../UserService";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";

interface Question {
    id: number,
    title: string,
    content: string,
    author: User,
    bestAnswer: Answer,
    answers: Answer[],
    createdAt: string,
    availableToEdit: boolean,
}

interface User {
    username: string
}

interface Answer {
    id: number,
    content: string,
    author: User,
    createdAt: string,
    availableToEdit: boolean,
}


const QuestionComponent: FC<{}> = () => {
    const {id} = useParams();
    const [question, setQuestion] = useState<Question>();
    const [answers, setAnswers] = useState<Answer[]>([]);
    const [replyField, setReplyField] = useState<string>('');
    const answersPerPage = 3;
    const navigate = useNavigate();

    useEffect(() => {
        getQuestion(id as string)
    }, [])

    function getQuestion(id: string) {
        trackPromise(getQuestionById(id), 'fetch-service')
            .then(r => {
                setQuestion(r?.data)
                setAnswers(r?.data.answers.slice(0, answersPerPage) as Answer[])
            }).catch(() => {
                navigate("/");
        });
    }

    function onReplyClickHandler() {
        postAnswer({
            content: replyField,
            questionId: question?.id
        }).then(r => {
            getQuestion(question?.id.toString() as string);
            setReplyField('');
        })
    }

    function onDeleteClickHandler() {
        deleteQuestion(id as string).then(() => {
            navigate('/');
        })
    }

    function onEditClickHandler() {
        navigate(`/question/edit/${id}`);
    }

    function onUpdateQuestion(body: any) {
        putQuestion(id as string, body)
            .then(() => {
                getQuestion(id as string);
            });
    }

    function renderAnswer(answer: Answer, color: string, isBest: boolean) {
        return (
            <Box key={answer.id} sx={{
                backgroundColor: color,
                padding: '10px',
                marginY: '10px'
            }}>
                <Typography>{answer?.content}</Typography>
                <Box mt="10px">
                    <Typography fontSize="0.8em">Created by: {answer?.author?.username}</Typography>
                    <Typography fontSize="0.8em">Created at: {answer?.createdAt}</Typography>
                </Box>
                {question?.availableToEdit && <Box sx={{
                    textAlign: 'right',
                }}>
                    {isBest
                        ? <Button size="small" onClick={() => onUpdateQuestion({
                            title: question?.title,
                            content: question?.content,
                            bestAnswerId: null,
                        })} variant="contained" sx={{
                            margin: '5px'
                        }}>Unmark best</Button>
                        : <Button size="small" onClick={() => onUpdateQuestion({
                            title: question?.title,
                            content: question?.content,
                            bestAnswerId: answer?.id,
                        })} variant="contained" sx={{
                            margin: '5px'
                        }}>Mark as best</Button>}
                </Box>}
                {answer?.availableToEdit && <Box>
                    <Button size="small"  onClick={() => console.log("da")} variant="contained" sx={{
                        margin: '5px'
                    }}>Edit</Button>
                    <Button size="small" onClick={() => console.log("da")} variant="contained" sx={{
                        margin: '5px'
                    }}>Delete</Button>
                </Box>}
            </Box>
        )
    }

    return (
        <Box padding="5px">
            <ProgressComponent/>
            {question &&
            <Box>
                <Box sx={{
                    borderBottom: '1px solid black'
                }}>
                    <Typography mx="20px" fontSize="2em">{question?.title}</Typography>
                    <Typography fontSize="0.8em" mx="20px">Created at: {question?.createdAt}</Typography>
                    <Box sx={{
                        padding: '20px',
                        borderTop: '1px solid black',
                    }}>
                        <Typography mx="20px" fontSize="1.2em">{question?.content}</Typography>
                    </Box>
                    <Typography mx="20px" fontSize="0.8em">Asked
                        by: {question?.author?.username}</Typography>
                    {question?.availableToEdit &&
                    <Box sx={{
                        my: '10px'
                    }}>
                        <Button size="small" onClick={onEditClickHandler} variant="contained" sx={{
                            marginX: '5px'
                        }}>Edit</Button>
                        <Button size="small" onClick={onDeleteClickHandler} variant="contained" sx={{
                            marginX: '5px'
                        }}>Delete</Button>
                    </Box>}
                </Box>
                <Grid container
                      alignItems="center"
                      justifyContent="center">
                    <Grid item md={6} sm={8} xs={10}>
                        {question?.bestAnswer &&
                        <Box mt="10px">
                            <Typography fontSize="1.4em">Best answer</Typography>
                            {renderAnswer(question?.bestAnswer, '#ccffe0', true)}
                        </Box>}
                        {answers?.length > 0 || question?.bestAnswer !== null
                            ?
                            <Box my="10px">
                                <Typography align="center">Other answers</Typography>
                                <Box sx={{
                                    display: 'flex',
                                    justifyContent: 'center'
                                }}>
                                    <Pagination count={Math.ceil(question?.answers.length as number / answersPerPage)}
                                                onChange={(e, v) => {
                                                    setAnswers(question?.answers?.slice((v - 1) * answersPerPage, answersPerPage * v) as Answer[])
                                                }} variant="outlined" color="primary"/>
                                </Box>
                            </Box>
                            : <Typography align="center" variant="h4">There are no answers yet</Typography>
                        }
                        {answers?.map(a => renderAnswer(a, '#cce4ff', false))}
                        {isAuthenticated() &&
                        <Grid container direction="column" justifyContent="center">
                            <Typography align="center">Your reply text</Typography>
                            <TextareaAutosize
                                value={replyField}
                                onChange={(e) => setReplyField(e.target.value)}
                                style={{
                                    marginBottom: '10px',
                                    minHeight: '100px',
                                    border: '2px solid #a9c9fc',
                                    backgroundColor: '#e6eefc',
                                    borderRadius: '5px',
                                }}/>
                            <Box margin="auto">
                                <Button size="small" variant="contained" onClick={onReplyClickHandler}>Reply</Button>
                            </Box>
                        </Grid>}
                    </Grid>
                </Grid>
            </Box>}
        </Box>)
}

export default QuestionComponent;