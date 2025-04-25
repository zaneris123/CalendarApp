import * as React from "react";
import {
  DateCalendar,
  LocalizationProvider,
  TimePicker,
  PickersDay,
} from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useState, useEffect } from "react";
import { useUserStore } from "../stores/user";
import { Box, Button, Card, Modal, TextField, Typography } from "@mui/material";
import { FetchEvents, postEvent, deleteEvent } from "../utils/events";
import dayjs from "dayjs";
import utc from "dayjs/plugin/utc";

export function Calendar() {
  const [selectedDate, setSelectedDate] = useState(dayjs()); // Initialize to the current date
  const [events, setEvents] = useState([]);
  const [open, setOpen] = useState(false);
  const [eventTitle, setEventTitle] = useState("");
  const [eventDescription, setEventDescription] = useState("");
  const [eventStartDate, setEventStartDate] = useState(null);
  const [eventEndDate, setEventEndDate] = useState(null);
  const user = useUserStore((state) => state.user);

  dayjs.extend(utc);

  const handleModalOpen = () => {
    setOpen(true);
  };
  const handleModalClose = () => {
    setOpen(false);
  };
  useEffect(() => {
    FetchEvents(user.id)
      .then((fetchedEvents) => {
        setEvents(fetchedEvents); 
      })
      .catch((error) => {
        console.error("Error fetching events:", error);
      });
  }, [user.id]);

  return (
    <div className="calendar-container">
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <DateCalendar
          value={selectedDate}
          onChange={(newValue) => {
            setSelectedDate(newValue);
          }}
          renderDay={(day, selectedDates, pickersDayProps) => {
            const isEvent = events.some(
              (event) => dayjs(event.startDate).format("YYYY-MM-DD") === day.format("YYYY-MM-DD")
            );
            return (
              <PickersDay
                {...pickersDayProps}
                day={day}
                sx={{
                  backgroundColor: isEvent ? "#ffeb3b" : undefined,
                  borderRadius: "50%",
                }}
              />
            );
          }}
        />
        <Button variant="contained" onClick={handleModalOpen}>
          {" "}
          New Event{" "}
        </Button>
        <Modal
          open={open}
          onClose={handleModalClose}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box
            sx={{
              width: 400,
              padding: 2,
              backgroundColor: "grey",
              margin: "auto",
              marginTop: "20vh",
            }}
          >
            <h2 id="modal-modal-title">Create New Event</h2>
            <TextField
              label="Event Title"
              variant="outlined"
              fullWidth
              value={eventTitle}
              onChange={(e) => setEventTitle(e.target.value)}
              error={eventTitle.length < 3 || eventTitle.length > 30 || !/^[\w\s.,'!&()\-]+$/.test(eventTitle)}
              helperText={
                eventTitle.length < 3
                  ? "Event title must be at least 3 characters long."
                  : eventTitle.length > 30
                  ? "Event title cannot exceed 30 characters."
                  : !/^[\w\s.,'!&()\-]+$/.test(eventTitle)
                  ? "Event title can contain letters, numbers, spaces, and common punctuation."
                  : ""
              }
            />
            <TextField
              label="Event Description"
              variant="outlined"
              fullWidth
              value={eventDescription}
              onChange={(e) => setEventDescription(e.target.value)}
              multiline
              error={eventDescription.length < 10 || eventDescription.length > 100 || !/^[\w\s.,'!&()\-]+$/.test(eventDescription)}
              helperText={
                eventDescription.length < 10
                  ? "Event title must be at least 10 characters long."
                  : eventDescription.length > 100
                  ? "Event title cannot exceed 100 characters."
                  : !/^[\w\s.,'!&()\-]+$/.test(eventDescription)
                  ? "Event title can contain letters, numbers, spaces, and common punctuation."
                  : ""
              }
            />
            <TimePicker
              label="Start Time"
              value={eventStartDate}
              onChange={(newValue) => setEventStartDate(newValue)}
            />
            <TimePicker
              label="End Time"
              value={eventEndDate}
              onChange={(newValue) => setEventEndDate(newValue)}
              minTime={eventStartDate}
            />
            <Button
              variant="contained"
              onClick={() => {
                const newEvent = {
                  title: eventTitle,
                  description: eventDescription,
                  startDate: dayjs(eventStartDate).utc(),
                  endDate: dayjs(eventEndDate).utc(),
                  userId: user.id,
                };

                postEvent(newEvent)
                  .then(() => FetchEvents(user.id))
                  .then((fetchedEvents) => {
                    setEvents([...fetchedEvents]); // Update the events list with a new array reference
                    setEventTitle(""); // Clear the form fields
                    setEventDescription("");
                    setEventStartDate(null);
                    setEventEndDate(null);
                    handleModalClose(); // Ensure modal closes after successful event creation
                  })
                  .catch((error) => {
                    console.error("Error creating event:", error);
                  });
              }}
            >
              Create Event
            </Button>
            <Button variant="outlined" onClick={handleModalClose}>
              Cancel
            </Button>
          </Box>
        </Modal>
        {events
        .filter((event) => {
          const eventDate = dayjs(event.startDate).format("YYYY-MM-DD");
          return eventDate === selectedDate.format("YYYY-MM-DD");
        }).map((event) => (
          <Card key={event.id} sx={{ margin: 2, padding: 2 }}>
            <Typography variant="h6">{event.title}</Typography>
            <Typography variant="body1">{event.description}</Typography>
            <Typography variant="body2">{dayjs(event.startDate).format("HH:mm")} - {dayjs(event.endDate).format("HH:mm")}</Typography>
            <Button variant="outlined" onClick={() => {
              deleteEvent(event.id)
              .then(() => FetchEvents(user.id))
              .then((fetchedEvents) => {
                setEvents([...fetchedEvents]); // Update the events list with a new array reference
              })
              .catch((error) => {
                console.error("Error deleting event:", error);
              });
            }}>
              Delete Event
            </Button>
          </Card>
        ))}
      </LocalizationProvider>
    </div>
  );
}
