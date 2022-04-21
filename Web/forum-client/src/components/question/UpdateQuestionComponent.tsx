import {FC, useEffect, useState} from "react";
import {Box, Button, FormControl, TextareaAutosize} from "@mui/material";
import {useNavigate, useParams} from "react-router-dom";
import Typography from "@mui/material/Typography";
import * as React from "react";
import {getQuestionById, putQuestion} from "../ApiService";
import {getUser} from "../UserService";

export interface UpdateQuestionRequest {
    title: string,
    content: string,
    bestAnswerId?: number
}

const UpdateQuestionComponent: FC<{}> = () => {

    const [updateQuestionRequest, setUpdateQuestionRequest] = useState<UpdateQuestionRequest>({
        title: '',
        content: '',
        bestAnswerId: 0,
    })

    const {id} = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        getQuestionById(id as string).then(r => {

            const data = r?.data;
            const title = data.title;
            const content = data?.content;
            const authorId = data?.author?.id;
            const bestAnswerId = data?.bestAnswer?.id;

            const user = getUser();
            if (user.id !== authorId) {
                navigate("/");
            }

            setUpdateQuestionRequest({
                bestAnswerId: bestAnswerId,
                title: title,
                content: content,
            })
        }).catch(() => {
            navigate("/");
        })
    }, [])

    function onFieldChange(field: string, value: any) {
        setUpdateQuestionRequest({...updateQuestionRequest, [field]: value} as UpdateQuestionRequest);
    }

    function onUpdateQuestionHandler() {
        putQuestion(id as string, updateQuestionRequest)
            .then(() => {
                navigate(`/question/${id}`);
            })
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            margin: '10px'
        }}>
            <FormControl sx={{padding: '10px'}}>
                <Typography variant="h4" mb="10px">Update existing question</Typography>
                <Typography>New title</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '50px',
                        width: '100%'
                    }}
                    required
                    value={updateQuestionRequest?.title}
                    onChange={(event) => onFieldChange("title", event.target.value)}/>
                <Typography>New content</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '100px',
                        width: '100%'
                    }}
                    required
                    value={updateQuestionRequest?.content}
                    onChange={(event) => onFieldChange("content", event.target.value)}/>
                <Button variant="contained" onClick={onUpdateQuestionHandler}>Update</Button>
            </FormControl>
        </Box>)
}

export default UpdateQuestionComponent;