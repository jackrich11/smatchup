export default function OwnedMatchup({ matchup, visitor }) {

    return (
        <>
        {matchup.isCreator && !visitor && 
        <p>Waiting for someone to join the matchup...</p>
        }
        {visitor && 
        <p>{visitor} has joined the matchup!</p>}
        </>
    )
}