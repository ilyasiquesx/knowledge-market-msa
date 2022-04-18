import * as React from 'react';
import Box from '@mui/material/Box';
import {FC, useEffect, useState} from "react";
import {Badge, CircularProgress} from "@mui/material";
import {usePromiseTracker} from "react-promise-tracker";

const ProgressComponent: FC<{}> = () => {
    const {promiseInProgress} = usePromiseTracker({area: 'fetch-service'});
    console.log(promiseInProgress);
    return (<Box>
        {promiseInProgress ? <Box sx={{
                display: 'flex',
                justifyContent: 'center'
            }}>
                <CircularProgress/>
            </Box>
            : null}
    </Box>)
}

export default ProgressComponent;