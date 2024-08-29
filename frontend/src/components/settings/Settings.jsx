export default function Settings({ settings }) {
    return (
    <>
    <p>Region: {settings.region}</p>
    <p>Skill Level: {settings.playerSkillLevel}</p>
    <p>Has Delay Mod: {settings.hasDelayMod ? "Yes" : "No"}</p>
    </>
    )
}