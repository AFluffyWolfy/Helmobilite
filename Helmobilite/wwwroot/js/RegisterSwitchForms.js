hideHelmoForm();
document.getElementById("role").value = "Client";

function switchForm() {
    if (document.getElementById("switchForm").checked) {
        showHelmoForm();
        hideClientForm();
        hideDriverForm();
        hideDispatcherForm();
        hideErrors();
        showDispatcherForm();
    } else {
        showClientForm();
        hideHelmoForm();
        hideErrors();

        document.getElementById("role").value = "Client";
    }
}
        
function hideErrors() {
    var spans = document.getElementsByClassName("text-danger");

    for (var i = 0; i < spans.length; i++) {
        var spansInside = spans[i].getElementsByTagName("span");
        for (var j = 0; j < spansInside.length; j += 1) {
            spansInside[j].remove();
        }
    };
}

function switchRole() {
    var radioButtonsHelmoRole = document.getElementsByName('helmoRole');
    let helmoRole;
    for (var i = 0; i < radioButtonsHelmoRole.length; i++) {
        if (radioButtonsHelmoRole[i].checked) {
            helmoRole = radioButtonsHelmoRole[i].value;
            break;
        }
    }

    if (helmoRole == "Dispatcher") {
        showDispatcherForm();
        hideDriverForm();
    } else if (helmoRole == "Driver") {
        showDriverForm();
        hideDispatcherForm();
    }
}

function hideHelmoForm() {
    let collection = document.getElementsByClassName("helmoForm");
    for (var i = 0; i < collection.length; i++) {
        collection.item(i).disabled = true;
        collection.item(i).style.display = 'none';
    }
}

function showHelmoForm() {
    let collection = document.getElementsByClassName("helmoForm");
    for (var i = 0; i < collection.length; i++) {
        collection.item(i).disabled = false;
        collection.item(i).style.display = 'block';
    }
}

function hideClientForm() {
    let collection = document.getElementsByClassName("clientForm");
    for (var i = 0; i < collection.length; i++) {
        collection.item(i).disabled = true;
        collection.item(i).style.display = 'none';
    }
}

function showClientForm() {
    let collection = document.getElementsByClassName("clientForm");
    for (var i = 0; i < collection.length; i++) {
        collection.item(i).disabled = false;
        collection.item(i).style.display = 'block';
    }
}

function hideDriverForm() {
    let input = document.getElementById("DriverLicense");
    input.disabled = true;
    input.style.display = 'none';
}

function showDriverForm() {
    let input = document.getElementById("DriverLicense");
    input.disabled = false;
    input.style.display = 'block';
    document.getElementById("role").value = "Driver";
}

function hideDispatcherForm() {
    let input = document.getElementById("StudyLevel");
    input.disabled = true;
    input.style.display = 'none';
}

function showDispatcherForm() {
    let input = document.getElementById("StudyLevel");
    input.disabled = false;
    input.style.display = 'block';
    document.getElementById("role").value = "Dispatcher";
}