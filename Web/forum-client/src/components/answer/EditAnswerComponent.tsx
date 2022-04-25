import {FC, useEffect, useState} from "react";
import {Box, Button, FormControl, TextareaAutosize} from "@mui/material";
import {useNavigate, useParams} from "react-router-dom";
import Typography from "@mui/material/Typography";
import * as React from "react";
import {getAnswerById, putAnswer} from "../ApiService";
import {getUser} from "../UserService";

export interface EditAnswerRequest {
    content: string,
}

const EditAnswerComponent: FC = () => {

    const [editAnswerRequest, setEditAnswerRequest] = useState<EditAnswerRequest>({
        content: '',
    })

    const {id, questionId} = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        getAnswerById(id as string).then(r => {

            const data = r?.data;
            const content = data?.content;
            const authorId = data?.authorId;

            const user = getUser();
            if (user.id !== authorId) {
                navigate("/");
            }

            setEditAnswerRequest({
                content: content,
            })
        }).catch(() => {
            navigate("/");
        })
    })

    function onFieldChange(field: string, value: any) {
        setEditAnswerRequest({...editAnswerRequest, [field]: value} as EditAnswerRequest);
    }

    function onUpdateAnswer() {
        putAnswer(id as string, editAnswerRequest)
            .then((r) => {
                if (r.status === 204) {
                    navigate(`/question/${questionId}`);
                }
            })
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            margin: '10px'
        }}>
            <FormControl sx={{padding: '10px'}}>
                <Typography variant="h4" mb="10px">Update existing answer</Typography>
                <Typography>New content</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '100px',
                        width: '100%'
                    }}
                    required
                    value={editAnswerRequest?.content}
                    onChange={(event) => onFieldChange("content", event.target.value)}/>
                <Button variant="contained" onClick={onUpdateAnswer}>Update</Button>
            </FormControl>
        </Box>)
}

export default EditAnswerComponent;