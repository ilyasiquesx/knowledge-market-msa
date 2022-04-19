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
            justifyContent: 'column',
            margin: '10px'
        }}>
            <FormControl sx={{padding: '10px'}}>
                <Typography align="center">Ask a new question</Typography>
                <TextField label="Title"
                           variant="filled"
                           required
                           value={createRequest?.title}
                           onChange={(event) => onFieldChange("title", event.target.value)}
                           sx={{marginY: '10px'}}>
                </TextField>
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
        </Box>)
}

export default CreateQuestionComponent;