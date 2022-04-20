import {FC, useEffect, useState} from "react";
import {Box, Button, Checkbox, FormControl, FormControlLabel, TextareaAutosize, TextField} from "@mui/material";
import {isAuthenticated} from "../UserService";
import {useNavigate} from "react-router-dom";
import Typography from "@mui/material/Typography";
import * as React from "react";
import {postQuestion} from "../ApiService";

interface CreateQuestionRequest {
    title: string,
    content: string,
}

const CreateQuestionComponent: FC<{ /*onUserChange: (user: User) => void*/ }> = (/*{onUserChange}*/) => {

    const [createRequest, setQuestionRequest] = useState<CreateQuestionRequest>({
        title: '',
        content: ''
    })

    const navigate = useNavigate();

    function onFieldChange(field: string, value: any) {
        setQuestionRequest({...createRequest, [field]: value} as CreateQuestionRequest);
    }

    function onCreateQuestionHandler() {
        postQuestion(createRequest)
            .then(() => {
                navigate('/');
            })
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            borderBottom: '1px solid black',
        }}>
            <FormControl sx={{
                padding: '10px',
            }}>
                <Typography variant="h4" sx={{
                    marginBottom: '10px'
                }}>Ask a new question</Typography>
                <Typography>Your title</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '50px',
                        width: '100%'
                    }}
                    required
                    value={createRequest?.title}
                    onChange={(event) => onFieldChange("title", event.target.value)}/>
                <Typography>Your content</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '100px',
                        width: '100%'
                    }}
                    required
                    value={createRequest?.content}
                    onChange={(event) => onFieldChange("content", event.target.value)}/>
                <Button variant="contained" onClick={onCreateQuestionHandler}>Ask</Button>
            </FormControl>
        </Box>
    )
}

export default CreateQuestionComponent;