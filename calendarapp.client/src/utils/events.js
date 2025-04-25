export const FetchEvents = (userId) => {
  return fetch(`http://localhost:5142/events/${userId}`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .catch((error) => {
      console.error("Error fetching events:", error);
      throw error;
    });
};

export const postEvent = (event) => {
    return fetch("http://localhost:5142/events", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(event),
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`HTTP error! status: ${response.status}, message: ${text}`);
            });
        }
        return response.json();
    });
};

export const deleteEvent = (eventId) => {
    return fetch(`http://localhost:5142/events/${eventId}`, {
        method: 'DELETE',
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`HTTP error! status: ${response.status}, message: ${text}`);
            });
        }
        return response.status === 204 ? null : response.json();
    });
}