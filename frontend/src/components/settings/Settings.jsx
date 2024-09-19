import { Stack, Typography } from "@mui/material";

export default function Settings({ settings }) {
    return (
    <>
    <Stack spacing={1}>
        <Typography>Region: {settings.region}</Typography>
        <Typography>Skill Level: {settings.playerSkillLevel}</Typography>
        <Typography>Has Delay Mod: {settings.hasDelayMod ? "Yes" : "No"}</Typography>
        </Stack>
    </>
    )
}

/**
 * TODO:
 * Continue to look for base react elements and replace them with MUI elements.
 * Use stacks to make things look neat, make buttons all the same variant and the correct color
 * Figure out how to make the website responsive
 */