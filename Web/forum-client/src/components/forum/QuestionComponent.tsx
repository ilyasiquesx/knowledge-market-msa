import {FC, useEffect, useState} from "react";
import Box from "@mui/material/Box";
import {useParams} from "react-router-dom"
import Typography from "@mui/material/Typography";
import {Pagination, TextField} from "@mui/material";
import {getQuestionById} from "../ApiService";
import Button from "@mui/material/Button";

interface Question {
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

    useEffect(() => {
        getQuestionById(id as string)
            .then(r => {
                setQuestion(r?.data)
                setAnswers(r?.data.answers.slice(0, 3) as Answer[])
            });
    }, [])

    console.log(question);
    console.log(answers);
    console.log(question?.answers.slice(0, 3) as Answer[])

    function renderAnswer(answer: Answer, color: string) {
        return (
            <Box key={answer.id} sx={{
                display: 'flex',
                justifyContent: 'center',
                flexDirection: 'column',
                gap: '5px',
                backgroundColor: color,
                padding: '5px',
                borderRadius: '5px'
            }}>
                <Typography>{answer?.content}</Typography>
                <Typography fontSize="0.8em" align="right">Created by: {answer?.author?.username}</Typography>
                <Typography fontSize="0.8em" align="right">Created at: {answer?.createdAt}</Typography>
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
            <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                padding: '10px',
                flexDirection: 'column',
                gap: '5px',
                marginBottom: '20px'
            }}>
                <Typography noWrap fontSize="2em" align="center" sx={{
                    borderRadius: '5px'
                }}>{question?.title}</Typography>
                <Typography fontSize="1.2em" align="center" sx={{
                    minWidth: '500px',
                    borderRadius: '5px'
                }}>{question?.content}</Typography>
                <Typography fontSize="0.8em" align="right">Asked by: {question?.author?.username}</Typography>
                <Typography fontSize="0.8em" align="right">Created at: {question?.createdAt}</Typography>
            </Box>
            {question?.bestAnswer && <Typography align="center">Best answer</Typography>}
            {question?.bestAnswer && renderAnswer(question?.bestAnswer, '#ccffe0')}
            {answers?.length > 0
                ? <Box sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center'
                }}> <Typography align="center">Other answers</Typography>
                    <Pagination count={Math.ceil(question?.answers.length as number / 3)} onChange={(e, v) => {
                        setAnswers(question?.answers?.slice((v - 1) * 3, 3 * v) as Answer[])
                    }} variant="outlined" color="primary"/>
                </Box>
                : <Typography align="center">There are no answers yet</Typography>
            }
            {answers?.map(a => renderAnswer(a, 'white'))}
            <Box sx={{
                display: 'flex',
            }}>
                <TextField sx={{
                    flexGrow: 1
                }}
                           id="full-width-text-field"
                           label="Type your answer"
                           placeholder="Type your answer"
                           helperText="Be polite..."
                />
            </Box>
            <Box margin="auto">
                <Button size="small" variant="contained">Reply</Button>
            </Box>
        </Box>)
}

export default QuestionComponent;