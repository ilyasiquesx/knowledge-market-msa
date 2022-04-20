import {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import {useNavigate, useParams} from "react-router-dom"
import Typography from "@mui/material/Typography";
import {Pagination, TextareaAutosize} from "@mui/material";
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
                display: 'flex',
                justifyContent: 'center',
                flexDirection: 'column',
                gap: '5px',
                backgroundColor: color,
                padding: '5px',
                borderRadius: '5px',
                border: '2px solid #a9c9fc',
            }}>
                <Typography>{answer?.content}</Typography>
                <Typography fontSize="0.8em" align="right">Created by: {answer?.author?.username}</Typography>
                <Typography fontSize="0.8em" align="right">Created at: {answer?.createdAt}</Typography>

                {question?.availableToEdit && <Box sx={{
                    textAlign: 'right',
                    mt: '5px'
                }}>
                    {isBest
                        ? <Button size="small" onClick={() => onUpdateQuestion({
                            title: question?.title,
                            content: question?.content,
                            bestAnswerId: null,
                        })} variant="contained" sx={{
                            marginX: '5px'
                        }}>Unmark best</Button>
                        : <Button size="small" onClick={() => onUpdateQuestion({
                            title: question?.title,
                            content: question?.content,
                            bestAnswerId: answer?.id,
                        })} variant="contained" sx={{
                            marginX: '5px'
                        }}>Mark as best</Button>}
                </Box>}
            </Box>
        )
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            margin: '10px',
            flexDirection: 'column',
            gap: '10px',
            maxWidth: '500px'
        }}>
            <ProgressComponent/>
            {question &&
            <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                padding: '10px',
                flexDirection: 'column',
                gap: '5px',
            }}>
                <Box sx={{
                    border: '2px solid #a9c9fc',
                    backgroundColor: '#e6eefc',
                    borderRadius: '5px',
                    padding: '5px',
                }}>
                    <Typography fontSize="2em" align="center" sx={{
                        borderRadius: '5px'
                    }}>{question?.title}</Typography>
                    <Typography fontSize="1.2em" align="center" sx={{
                        minWidth: '320px',
                        borderRadius: '5px'
                    }}>{question?.content}</Typography>
                    <Typography fontSize="0.8em" align="right" marginTop="10px">Asked
                        by: {question?.author?.username}</Typography>
                    <Typography fontSize="0.8em" align="right">Created at: {question?.createdAt}</Typography>
                    {question?.availableToEdit &&
                    <Box sx={{
                        textAlign: 'right',
                        mt: '5px'
                    }}>
                        <Button size="small" onClick={onEditClickHandler} variant="contained" sx={{
                            marginX: '5px'
                        }}>Edit</Button>
                        <Button size="small" onClick={onDeleteClickHandler} variant="contained" sx={{
                            marginX: '5px'
                        }}>Delete</Button>
                    </Box>}
                </Box>
                {question?.bestAnswer &&
                <Box>
                    <Typography align="center">Best answer</Typography>
                    {renderAnswer(question?.bestAnswer, '#ccffe0', true)}
                </Box>}
                {answers?.length > 0
                    ? <Box sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center'
                    }}> <Typography align="center">Other answers</Typography>
                        <Pagination count={Math.ceil(question?.answers.length as number / answersPerPage)}
                                    onChange={(e, v) => {
                                        setAnswers(question?.answers?.slice((v - 1) * answersPerPage, answersPerPage * v) as Answer[])
                                    }} variant="outlined" color="primary"/>
                    </Box>
                    : <Typography align="center" variant="h4">There are no answers yet</Typography>
                }
                {answers?.map(a => renderAnswer(a, '#cce4ff', false))}
                {isAuthenticated() &&
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'column'
                }}>
                    <Typography align="center">Your reply text</Typography>
                    <TextareaAutosize
                        value={replyField}
                        onChange={(e) => setReplyField(e.target.value)}
                        style={{
                            marginBottom: '10px',
                            width: '100%',
                            minHeight: '100px',
                            border: '2px solid #a9c9fc',
                            backgroundColor: '#e6eefc',
                            borderRadius: '5px',
                        }}/>
                    <Box margin="auto">
                        <Button size="small" variant="contained" onClick={onReplyClickHandler}>Reply</Button>
                    </Box>
                </Box>}
            </Box>}
        </Box>)
}

export default QuestionComponent;