const api = {
	token: sessionStorage.getItem("dd-token"),
	userName: sessionStorage.getItem("dd-user"),
	isRegistering: false,
	characters: [],
	monsters: [],
	swords: [],
	selectedCharacterId: sessionStorage.getItem("dd-character"),
	selectedMonsterId: sessionStorage.getItem("dd-monster"),
};

const byId = (id) => document.getElementById(id);

function setStatus(text, online = true) {
	byId("connection-status").textContent = text;
	document.querySelector(".status-dot").style.background = online
		? "var(--green)"
		: "var(--red)";
}

function showMessage(message, error = false) {
	const output = byId("game-message");
	output.textContent = message;
	output.style.color = error ? "var(--red)" : "var(--orange)";
}

function setLoggedIn(token, userName) {
	api.token = token;
	api.userName = userName;
	sessionStorage.setItem("dd-token", token);
	sessionStorage.setItem("dd-user", userName);
	byId("auth-screen").hidden = true;
	byId("game-screen").hidden = false;
	byId("logout-button").hidden = false;
	byId("player-name").textContent = userName;
	setStatus("CONNECTED");
	refreshGame();
}

function logOut() {
	sessionStorage.removeItem("dd-token");
	sessionStorage.removeItem("dd-user");
	sessionStorage.removeItem("dd-character");
	api.token = null;
	api.userName = null;
	api.selectedCharacterId = null;
	byId("game-screen").hidden = true;
	byId("auth-screen").hidden = false;
	byId("logout-button").hidden = true;
	byId("auth-error").textContent = "";
	setStatus("SYSTEM READY");
}

async function request(path, options = {}) {
	const headers = new Headers(options.headers || {});
	if (options.body) headers.set("Content-Type", "application/json");
	if (api.token) headers.set("Authorization", `Bearer ${api.token}`);

	const response = await fetch(path, { ...options, headers });
	if (response.status === 401 && api.token) {
		logOut();
		throw new Error("Your session expired. Please log in again.");
	}

	const contentType = response.headers.get("content-type") || "";
	const body = contentType.includes("application/json")
		? await response.json()
		: await response.text();
	if (!response.ok) {
		const message =
			typeof body === "string" ? body : body.error || body.title;
		throw new Error(
			message || `Request failed (${response.status}).`,
		);
	}
	return body;
}

byId("auth-form").addEventListener("submit", async (event) => {
	event.preventDefault();
	const error = byId("auth-error");
	error.textContent = "";
	const userName = byId("username").value.trim();
	const password = byId("password").value;
	const endpoint = api.isRegistering
		? "/auth/register"
		: "/auth/login";
	byId("auth-submit").disabled = true;
	try {
		const result = await request(endpoint, {
			method: "POST",
			body: JSON.stringify({ userName, password }),
		});
		setLoggedIn(result.token, result.userName);
	} catch (exception) {
		error.textContent = exception.message;
	} finally {
		byId("auth-submit").disabled = false;
	}
});

byId("switch-auth").addEventListener("click", () => {
	api.isRegistering = !api.isRegistering;
	byId("auth-title").textContent = api.isRegistering
		? "Create account"
		: "Log in";
	byId("auth-mode-label").textContent = api.isRegistering
		? "NEW PLAYER REGISTRATION"
		: "PLAYER ACCESS";
	byId("auth-submit").textContent = api.isRegistering
		? "CREATE ACCOUNT"
		: "ENTER THE DUNGEON";
	byId("switch-prompt").textContent = api.isRegistering
		? "Already have an account?"
		: "New around here?";
	byId("switch-auth").textContent = api.isRegistering
		? "Log in"
		: "Create an account";
	byId("password").autocomplete = api.isRegistering
		? "new-password"
		: "current-password";
	byId("password").minLength = api.isRegistering ? 5 : 1;
	byId("password-hint").hidden = !api.isRegistering;
	byId("auth-error").textContent = "";
});

byId("logout-button").addEventListener("click", logOut);

function addOption(select, value, label) {
	const option = document.createElement("option");
	option.value = value;
	option.textContent = label;
	select.append(option);
}

function renderCharacters() {
	const list = byId("character-list");
	const select = byId("character-select");
	list.replaceChildren();
	select.replaceChildren();
	byId("character-count").textContent = api.characters.length;

	if (!api.characters.length) {
		const empty = document.createElement("p");
		empty.className = "card-stats";
		empty.textContent = "No adventurers yet. Recruit one below.";
		list.append(empty);
		addOption(select, "", "Create a character first");
		api.selectedCharacterId = null;
		return;
	}

	if (
		!api.characters.some(
			(character) => character.id === api.selectedCharacterId,
		)
	)
		api.selectedCharacterId = api.characters[0].id;

	for (const character of api.characters) {
		addOption(
			select,
			character.id,
			`${character.name} · LV ${character.level}`,
		);
		const card = document.createElement("article");
		card.className = `character-card${character.id === api.selectedCharacterId ? " selected" : ""}`;
		card.tabIndex = 0;
		card.setAttribute("role", "button");
		card.setAttribute(
			"aria-pressed",
			character.id === api.selectedCharacterId,
		);
		const title = document.createElement("div");
		title.className = "card-title";
		const name = document.createElement("span");
		name.textContent = character.name;
		const level = document.createElement("small");
		level.textContent = `LV ${character.level}`;
		title.append(name, level);
		const stats = document.createElement("div");
		stats.className = "card-stats";
		stats.textContent = `HP ${character.hp}  /  DMG ${character.damage}`;
		const track = document.createElement("div");
		track.className = "hp-track";
		const fill = document.createElement("div");
		fill.className = "hp-fill";
		fill.style.width = `${Math.max(0, Math.min(100, (character.hp / Math.max(1, character.level * 300)) * 100))}%`;
		track.append(fill);
		card.append(title, stats, track);
		card.addEventListener("click", () =>
			selectCharacter(character.id),
		);
		card.addEventListener("keydown", (event) => {
			if (event.key === "Enter" || event.key === " ")
				selectCharacter(character.id);
		});
		list.append(card);
	}
	select.value = api.selectedCharacterId;
}

function selectCharacter(id) {
	api.selectedCharacterId = id;
	sessionStorage.setItem("dd-character", id);
	renderCharacters();
}

function renderMonsters() {
	const list = byId("monster-list");
	const select = byId("monster-select");
	list.replaceChildren();
	select.replaceChildren();
	byId("monster-count").textContent = api.monsters.length;
	const available = api.monsters.filter(
		(monster) => monster.quantity > 0,
	);
	if (!available.some((monster) => monster.id === api.selectedMonsterId))
		api.selectedMonsterId = available[0]?.id || null;
	if (!available.length)
		addOption(select, "", "No monsters available");

	for (const monster of api.monsters) {
		const card = document.createElement("article");
		card.className = "monster-card";
		const title = document.createElement("div");
		title.className = "card-title";
		const name = document.createElement("span");
		name.textContent = monster.name;
		const type = document.createElement("small");
		type.textContent = monster.typeOfMonster;
		title.append(name, type);
		const stats = document.createElement("div");
		stats.className = "card-stats";
		stats.textContent = `HP ${monster.hp} · DMG ${monster.damage} · XP ${monster.xpReward} · QTY ${monster.quantity}`;
		card.append(title, stats);
		list.append(card);
		if (monster.quantity > 0)
			addOption(
				select,
				monster.id,
				`${monster.name} · HP ${monster.hp}`,
			);
	}
	select.value = api.selectedMonsterId || "";
}

byId("monster-select").addEventListener("change", (event) => {
	api.selectedMonsterId = event.target.value;
	if (api.selectedMonsterId)
		sessionStorage.setItem("dd-monster", api.selectedMonsterId);
	else sessionStorage.removeItem("dd-monster");
});

function renderSwords() {
	const select = byId("sword-select");
	const equipped = byId("equipped-sword");
	select.replaceChildren();
	const character = api.characters.find(
		(item) => item.id === api.selectedCharacterId,
	);
	const equippedSword = api.swords.find(
		(sword) => sword.id === character?.swordId,
	);
	equipped.textContent = equippedSword
		? `Equipped: ${equippedSword.name} · +${equippedSword.damage} DMG`
		: "Equipped: none";
	if (!api.swords.length) {
		addOption(select, "", "No swords available");
		byId("equip-button").disabled = true;
	}
	for (const sword of api.swords)
		addOption(
			select,
			sword.id,
			`${sword.name} · +${sword.damage} DMG`,
		);
	if (api.swords.length) {
		select.value = equippedSword?.id || api.swords[0].id;
		byId("equip-button").disabled = false;
	}
}

async function refreshGame() {
	setStatus("SYNCING…");
	try {
		const [characters, monsters, swords] = await Promise.all([
			request("/StoreCharacters"),
			request("/StoreMonsters?page=1&pageSize=100"),
			request("/StoreSwords"),
		]);
		api.characters = characters;
		api.monsters = monsters;
		api.swords = swords;
		renderCharacters();
		renderMonsters();
		renderSwords();
		setStatus("CONNECTED");
	} catch (exception) {
		setStatus("OFFLINE", false);
		showMessage(exception.message, true);
	}
}

byId("new-character-form").addEventListener(
	"submit",
	async (event) => {
		event.preventDefault();
		const input = byId("new-character-name");
		try {
			const character = await request("/StoreCharacters", {
				method: "POST",
				body: JSON.stringify({ name: input.value.trim() }),
			});
			input.value = "";
			api.characters.push(character);
			selectCharacter(character.id);
			showMessage(`${character.name} has joined the party.`);
		} catch (exception) {
			showMessage(exception.message, true);
		}
	},
);

byId("character-select").addEventListener("change", (event) =>
	{
		selectCharacter(event.target.value);
		renderSwords();
	},
);

byId("fight-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	const monsterId = byId("monster-select").value;
	if (!characterId || !monsterId)
		return showMessage(
			"Create a character and choose an available monster first.",
			true,
		);
	const button = byId("fight-button");
	button.disabled = true;
	try {
		const result = await request("/Fight", {
			method: "POST",
			body: JSON.stringify({ characterId, monsterId }),
		});
		const log = byId("battle-log");
		log.replaceChildren();
		for (const line of result.battleLog || []) {
			const entry = document.createElement("p");
			entry.textContent = `> ${line}`;
			if (/defeat|wins|victory|level up/i.test(line))
				entry.className = "log-win";
			if (/damage|died|defeated/i.test(line))
				entry.className = "log-danger";
			log.append(entry);
			log.scrollTop = log.scrollHeight;
			await new Promise((resolve) => setTimeout(resolve, 450));
		}
		const summary = document.createElement("p");
		summary.className = result.characterWon
			? "log-win"
			: "log-danger";
		summary.textContent = `> ${result.message || (result.characterWon ? "Victory." : "Defeat.")}`;
		log.append(summary);
		log.scrollTop = log.scrollHeight;
		showMessage(result.message || "Fight complete.");
		await refreshGame();
	} catch (exception) {
		showMessage(exception.message, true);
	} finally {
		button.disabled = false;
	}
});

byId("camp-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	if (!characterId)
		return showMessage("Choose a character first.", true);
	try {
		const result = await request(
			`/StoreCharacters/${characterId}/camp`,
			{ method: "POST" },
		);
		showMessage(result.message || `Rested. HP is now ${result.hp}.`);
		await refreshGame();
	} catch (exception) {
		showMessage(exception.message, true);
	}
});

byId("equip-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	const swordId = byId("sword-select").value;
	if (!characterId || !swordId)
		return showMessage("Choose a character and sword first.", true);
	try {
		const result = await request(
			`/StoreCharacters/${characterId}/equipment/${swordId}`,
			{ method: "PUT" },
		);
		showMessage(
			`${result.swordName} equipped (+${result.swordDamage} damage).`,
		);
		await refreshGame();
	} catch (exception) {
		showMessage(exception.message, true);
	}
});

async function loadMusic() {
	const select = byId("music-track");
	try {
		const response = await fetch("/audio/tracks.json", {
			cache: "no-store",
		});
		if (!response.ok) throw new Error("Track list unavailable");
		const tracks = await response.json();
		select.replaceChildren();
		if (!tracks.length)
			return addOption(select, "", "Add tracks to wwwroot/audio");
		for (const track of tracks)
			addOption(select, track.file, track.title);
		select.dispatchEvent(new Event("change"));
	} catch {
		select.replaceChildren();
		addOption(select, "", "No tracks configured");
	}
}

const audio = byId("game-music");
byId("music-track").addEventListener("change", (event) => {
	const file = event.target.value;
	audio.pause();
	audio.src = file ? `/audio/${encodeURIComponent(file)}` : "";
	byId("music-toggle").textContent = "▶";
});
byId("music-toggle").addEventListener("click", async () => {
	if (!audio.src)
		return showMessage(
			"Add your audio files and list them in wwwroot/audio/tracks.json.",
			true,
		);
	if (audio.paused) {
		try {
			await audio.play();
			byId("music-toggle").textContent = "Ⅱ";
		} catch {
			showMessage(
				"The browser could not play this audio file.",
				true,
			);
		}
	} else {
		audio.pause();
		byId("music-toggle").textContent = "▶";
	}
});
byId("music-volume").addEventListener("input", (event) => {
	audio.volume = Number(event.target.value);
});
audio.volume = Number(byId("music-volume").value);

loadMusic();
if (api.token && api.userName) setLoggedIn(api.token, api.userName);
